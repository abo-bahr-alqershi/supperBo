#!/usr/bin/env node

import fs from 'fs';
import path from 'path';
import { execSync } from 'child_process';
import { fileURLToPath } from 'url';

// للحصول على __dirname في ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// معالجة خيارات سطر الأوامر
function parseArguments() {
  const args = process.argv.slice(2);
  let projectPath = '.'; // المجلد الحالي كافتراضي
  let options = {
    verbose: false,
    outputFile: null,
    includeTests: false
  };

  for (let i = 0; i < args.length; i++) {
    const arg = args[i];
    
    switch (arg) {
      case '-h':
      case '--help':
        showHelp();
        process.exit(0);
        break;
      case '-v':
      case '--verbose':
        options.verbose = true;
        break;
      case '-o':
      case '--output':
        options.outputFile = args[++i];
        break;
      case '--include-tests':
        options.includeTests = true;
        break;
      case '-p':
      case '--project':
        projectPath = args[++i];
        break;
      default:
        // إذا لم يكن خيار، اعتبره مسار المشروع
        if (!arg.startsWith('-')) {
          projectPath = arg;
        }
    }
  }

  return { projectPath, options };
}

function showHelp() {
  console.log(`
🔍 محلل الكود غير المستخدم - React TypeScript

الاستخدام:
  node unused-code-analyzer.js [المسار] [الخيارات]

الأمثلة:
  node unused-code-analyzer.js                    # تحليل المجلد الحالي
  node unused-code-analyzer.js ./my-project       # تحليل مشروع محدد
  node unused-code-analyzer.js -p ./src           # تحليل مجلد src فقط
  node unused-code-analyzer.js --verbose          # تشغيل مفصل
  node unused-code-analyzer.js -o report.json     # حفظ التقرير في ملف محدد

الخيارات:
  -p, --project <path>     مسار المشروع (افتراضي: المجلد الحالي)
  -v, --verbose           عرض تفاصيل أكثر أثناء التحليل
  -o, --output <file>     ملف حفظ التقرير (افتراضي: unused-code-report.json)
  --include-tests         تضمين ملفات الاختبار في التحليل
  -h, --help              عرض هذه الرسالة

المجلدات المدعومة:
  - src/                  مجلد الكود المصدري
  - components/           مجلد المكونات
  - hooks/               مجلد الـ hooks
  - services/            مجلد الخدمات
  - utils/               مجلد الأدوات المساعدة
  `);
}

class UnusedCodeAnalyzer {
  constructor(projectRoot = '.', options = {}) {
    this.projectRoot = path.resolve(projectRoot);
    this.options = options;
    
    // البحث عن مجلد src أو استخدام المجلد الجذر
    const srcPath = path.join(this.projectRoot, 'src');
    this.srcPath = fs.existsSync(srcPath) ? srcPath : this.projectRoot;
    
    if (this.options.verbose) {
      console.log(`📁 مجلد المشروع: ${this.projectRoot}`);
      console.log(`📂 مجلد الكود المصدري: ${this.srcPath}`);
    }

    this.results = {
      unusedFiles: [],
      unusedExports: [],
      unusedTypes: [],
      unusedHooks: [],
      unusedServices: [],
      unusedComponents: [],
      deadCode: [],
      summary: {},
      projectInfo: {
        projectPath: this.projectRoot,
        srcPath: this.srcPath,
        analyzedAt: new Date().toISOString()
      }
    };
    
    // أنماط الملفات المدعومة
    this.fileExtensions = ['.ts', '.tsx', '.js', '.jsx'];
    
    // أنماط التجاهل
    this.ignorePatterns = [
      /node_modules/,
      /\.d\.ts$/,
      /\.config\./,
      /dist\//,
      /build\//,
      /__snapshots__/
    ];

    // إضافة أنماط ملفات الاختبار إذا لم تكن مُضمنة
    if (!this.options.includeTests) {
      this.ignorePatterns.push(
        /\.test\./,
        /\.spec\./,
        /\.stories\./,
        /__tests__/
      );
    }

    this.allFiles = [];
    this.fileContents = new Map();
    this.imports = new Map();
    this.exports = new Map();
  }

  // قراءة جميع الملفات في المشروع
  async scanProject() {
    console.log('🔍 بدء فحص المشروع...');
    this.allFiles = this.getAllFiles(this.srcPath);
    
    // قراءة محتوى جميع الملفات
    for (const file of this.allFiles) {
      try {
        const content = fs.readFileSync(file, 'utf-8');
        this.fileContents.set(file, content);
      } catch (error) {
        console.warn(`⚠️  خطأ في قراءة الملف: ${file}`);
      }
    }
    
    console.log(`✅ تم فحص ${this.allFiles.length} ملف`);
  }

  // الحصول على جميع الملفات
  getAllFiles(dir) {
    const files = [];
    
    try {
      const items = fs.readdirSync(dir);
      
      for (const item of items) {
        const fullPath = path.join(dir, item);
        
        if (this.shouldIgnore(fullPath)) continue;
        
        if (fs.statSync(fullPath).isDirectory()) {
          files.push(...this.getAllFiles(fullPath));
        } else if (this.isValidFile(fullPath)) {
          files.push(fullPath);
        }
      }
    } catch (error) {
      console.warn(`⚠️  خطأ في قراءة المجلد: ${dir}`);
    }
    
    return files;
  }

  // التحقق من صحة الملف
  isValidFile(filePath) {
    return this.fileExtensions.some(ext => filePath.endsWith(ext));
  }

  // التحقق من أنماط التجاهل
  shouldIgnore(filePath) {
    return this.ignorePatterns.some(pattern => pattern.test(filePath));
  }

  // تحليل الـ imports والـ exports
  analyzeImportsExports() {
    console.log('🔍 تحليل الـ imports والـ exports...');
    
    for (const [filePath, content] of this.fileContents) {
      this.extractImports(filePath, content);
      this.extractExports(filePath, content);
    }
  }

  // استخراج الـ imports
  extractImports(filePath, content) {
    const imports = [];
    
    // Import statements عادية
    const importRegex = /import\s+(?:(?:\{[^}]*\}|\*\s+as\s+\w+|\w+)(?:\s*,\s*\{[^}]*\})?\s+from\s+)?['"`]([^'"`]+)['"`]/g;
    let match;
    
    while ((match = importRegex.exec(content)) !== null) {
      imports.push({
        source: match[1],
        line: this.getLineNumber(content, match.index)
      });
    }
    
    // Dynamic imports
    const dynamicImportRegex = /import\s*\(\s*['"`]([^'"`]+)['"`]\s*\)/g;
    while ((match = dynamicImportRegex.exec(content)) !== null) {
      imports.push({
        source: match[1],
        line: this.getLineNumber(content, match.index),
        dynamic: true
      });
    }
    
    this.imports.set(filePath, imports);
  }

  // استخراج الـ exports
  extractExports(filePath, content) {
    const exports = [];
    
    // Named exports
    const namedExportRegex = /export\s+(?:const|let|var|function|class|interface|type|enum)\s+(\w+)/g;
    let match;
    
    while ((match = namedExportRegex.exec(content)) !== null) {
      exports.push({
        name: match[1],
        type: 'named',
        line: this.getLineNumber(content, match.index)
      });
    }
    
    // Default exports
    const defaultExportRegex = /export\s+default\s+(?:(?:function|class)\s+(\w+)|(\w+))/g;
    while ((match = defaultExportRegex.exec(content)) !== null) {
      exports.push({
        name: match[1] || match[2] || 'default',
        type: 'default',
        line: this.getLineNumber(content, match.index)
      });
    }
    
    // Export from
    const exportFromRegex = /export\s+(?:\{[^}]*\}|\*)\s+from\s+['"`]([^'"`]+)['"`]/g;
    while ((match = exportFromRegex.exec(content)) !== null) {
      exports.push({
        name: 'reexport',
        type: 'reexport',
        source: match[1],
        line: this.getLineNumber(content, match.index)
      });
    }
    
    this.exports.set(filePath, exports);
  }

  // الحصول على رقم السطر
  getLineNumber(content, index) {
    return content.substring(0, index).split('\n').length;
  }

  // البحث عن الـ hooks غير المستخدمة
  findUnusedHooks() {
    console.log('🎣 البحث عن الـ hooks غير المستخدمة...');
    
    const hookFiles = this.allFiles.filter(file => 
      file.includes('hook') || file.includes('use') || /use[A-Z]/.test(path.basename(file))
    );
    
    for (const hookFile of hookFiles) {
      const relativePath = path.relative(this.projectRoot, hookFile);
      const content = this.fileContents.get(hookFile);
      
      if (!content) continue;
      
      // استخراج أسماء الـ hooks
      const hookExports = this.exports.get(hookFile) || [];
      const usedInFiles = this.findUsageInProject(hookFile);
      
      if (usedInFiles.length === 0) {
        this.results.unusedHooks.push({
          file: relativePath,
          exports: hookExports,
          reason: 'لا يتم استخدام هذا الـ hook في أي مكان في المشروع'
        });
      }
    }
  }

  // البحث عن الخدمات غير المستخدمة
  findUnusedServices() {
    console.log('🛠️  البحث عن الخدمات غير المستخدمة...');
    
    const serviceFiles = this.allFiles.filter(file => 
      file.includes('service') || file.includes('api') || file.includes('util')
    );
    
    for (const serviceFile of serviceFiles) {
      const relativePath = path.relative(this.projectRoot, serviceFile);
      const content = this.fileContents.get(serviceFile);
      
      if (!content) continue;
      
      // تحليل الدوال المُصدَّرة وغير المُصدَّرة
      const unusedFunctions = this.findUnusedFunctionsInFile(serviceFile, content);
      
      if (unusedFunctions.exported.length > 0 || unusedFunctions.internal.length > 0) {
        this.results.unusedServices.push({
          file: relativePath,
          unusedExportedFunctions: unusedFunctions.exported,
          unusedInternalFunctions: unusedFunctions.internal,
          reason: 'يحتوي على دوال غير مستخدمة'
        });
      }
      
      // التحقق من عدم استخدام الملف بالكامل
      const usedInFiles = this.findUsageInProject(serviceFile);
      if (usedInFiles.length === 0) {
        this.results.unusedServices.push({
          file: relativePath,
          reason: 'لا يتم استخدام هذه الخدمة في أي مكان في المشروع',
          completelyUnused: true
        });
      }
    }
  }

  // البحث عن الدوال غير المستخدمة في ملف واحد
  findUnusedFunctionsInFile(filePath, content) {
    const result = {
      exported: [],
      internal: []
    };
    
    // استخراج جميع الدوال
    const functionRegex = /(?:export\s+)?(?:const|let|var|function|async\s+function)\s+(\w+)\s*(?:=|:|=\s*(?:async\s*)?\()/g;
    const arrowFunctionRegex = /(?:export\s+)?(?:const|let|var)\s+(\w+)\s*=\s*(?:async\s*)?\(/g;
    
    let match;
    const allFunctions = [];
    
    // جمع الدوال العادية
    while ((match = functionRegex.exec(content)) !== null) {
      const isExported = match[0].includes('export');
      allFunctions.push({
        name: match[1],
        isExported,
        line: this.getLineNumber(content, match.index)
      });
    }
    
    // جمع Arrow functions
    content.replace(functionRegex, ''); // إزالة المطابقات السابقة
    while ((match = arrowFunctionRegex.exec(content)) !== null) {
      const isExported = match[0].includes('export');
      allFunctions.push({
        name: match[1],
        isExported,
        line: this.getLineNumber(content, match.index)
      });
    }
    
    // فحص استخدام كل دالة
    for (const func of allFunctions) {
      let isUsed = false;
      
      if (func.isExported) {
        // للدوال المُصدَّرة: البحث في الملفات الأخرى
        isUsed = this.isFunctionUsedExternally(func.name, filePath);
      } else {
        // للدوال الداخلية: البحث في نفس الملف
        isUsed = this.isFunctionUsedInternally(func.name, content, func.line);
      }
      
      if (!isUsed) {
        if (func.isExported) {
          result.exported.push({
            name: func.name,
            line: func.line
          });
        } else {
          result.internal.push({
            name: func.name,
            line: func.line
          });
        }
      }
    }
    
    return result;
  }
  
  // التحقق من استخدام الدالة خارجياً
  isFunctionUsedExternally(functionName, sourceFile) {
    for (const [filePath, content] of this.fileContents) {
      if (filePath === sourceFile) continue;
      
      // البحث عن استيراد الدالة
      const importRegex = new RegExp(`\\{[^}]*\\b${functionName}\\b[^}]*\\}`, 'g');
      if (importRegex.test(content)) {
        // التأكد من استخدامها بعد الاستيراد
        const usageRegex = new RegExp(`\\b${functionName}\\s*\\(`, 'g');
        if (usageRegex.test(content)) {
          return true;
        }
      }
    }
    return false;
  }
  
  // التحقق من استخدام الدالة داخلياً
  isFunctionUsedInternally(functionName, content, definitionLine) {
    const lines = content.split('\n');
    
    for (let i = 0; i < lines.length; i++) {
      if (i + 1 === definitionLine) continue; // تجاهل سطر التعريف
      
      const usageRegex = new RegExp(`\\b${functionName}\\s*\\(`, 'g');
      if (usageRegex.test(lines[i])) {
        return true;
      }
    }
    
    return false;
  }

  // البحث عن الأنواع غير المستخدمة
  findUnusedTypes() {
    console.log('📝 البحث عن الأنواع غير المستخدمة...');
    
    for (const [filePath, content] of this.fileContents) {
      const relativePath = path.relative(this.projectRoot, filePath);
      
      // استخراج تعريفات الأنواع
      const typeRegex = /(?:export\s+)?(?:type|interface)\s+(\w+)/g;
      let match;
      
      while ((match = typeRegex.exec(content)) !== null) {
        const typeName = match[1];
        const usageCount = this.countTypeUsage(typeName, filePath);
        
        if (usageCount === 0) {
          this.results.unusedTypes.push({
            file: relativePath,
            type: typeName,
            line: this.getLineNumber(content, match.index),
            reason: 'لا يتم استخدام هذا النوع في أي مكان'
          });
        }
      }
    }
  }

  // عد استخدام النوع
  countTypeUsage(typeName, excludeFile) {
    let count = 0;
    
    for (const [filePath, content] of this.fileContents) {
      if (filePath === excludeFile) continue;
      
      // البحث عن استخدامات النوع
      const usageRegex = new RegExp(`\\b${typeName}\\b`, 'g');
      const matches = content.match(usageRegex) || [];
      count += matches.length;
    }
    
    return count;
  }

  // البحث عن استخدام الملف في المشروع
  findUsageInProject(targetFile) {
    const usedInFiles = [];
    const fileName = path.basename(targetFile, path.extname(targetFile));
    const relativePath = path.relative(this.srcPath, targetFile);
    
    for (const [filePath, imports] of this.imports) {
      if (filePath === targetFile) continue;
      
      for (const importItem of imports) {
        if (this.isImportingFile(importItem.source, targetFile, filePath)) {
          usedInFiles.push({
            file: path.relative(this.projectRoot, filePath),
            line: importItem.line
          });
        }
      }
    }
    
    return usedInFiles;
  }

  // التحقق من استيراد الملف
  isImportingFile(importSource, targetFile, importingFile) {
    if (importSource.startsWith('.')) {
      // Relative import
      const resolvedPath = path.resolve(path.dirname(importingFile), importSource);
      const targetPath = targetFile.replace(/\.[^.]+$/, '');
      
      return resolvedPath === targetPath || 
             resolvedPath + '.ts' === targetFile ||
             resolvedPath + '.tsx' === targetFile ||
             resolvedPath + '.js' === targetFile ||
             resolvedPath + '.jsx' === targetFile;
    }
    
    return false;
  }

  // البحث عن المكونات غير المستخدمة
  findUnusedComponents() {
    console.log('⚛️  البحث عن المكونات غير المستخدمة...');
    
    const componentFiles = this.allFiles.filter(file => {
      const fileName = path.basename(file);
      return fileName[0] === fileName[0].toUpperCase() || 
             file.includes('component') ||
             file.includes('Component');
    });
    
    for (const componentFile of componentFiles) {
      const relativePath = path.relative(this.projectRoot, componentFile);
      const usedInFiles = this.findUsageInProject(componentFile);
      
      // تجاهل الملفات التي تحتوي على صفحات أو routes
      if (componentFile.includes('page') || 
          componentFile.includes('Page') ||
          componentFile.includes('route') ||
          componentFile.includes('Route')) {
        continue;
      }
      
      if (usedInFiles.length === 0) {
        this.results.unusedComponents.push({
          file: relativePath,
          reason: 'لا يتم استخدام هذا المكون في أي مكان في المشروع'
        });
      }
    }
  }

  // تشغيل التحليل الكامل
  async analyze() {
    console.log('🚀 بدء التحليل الشامل للمشروع...\n');
    
    try {
      await this.scanProject();
      this.analyzeImportsExports();
      this.findUnusedHooks();
      this.findUnusedServices();
      this.findUnusedTypes();
      this.findUnusedComponents();
      
      this.generateSummary();
      this.generateReport();
      
    } catch (error) {
      console.error('❌ خطأ في التحليل:', error.message);
    }
  }

  // إنتاج الملخص
  generateSummary() {
    this.results.summary = {
      totalFiles: this.allFiles.length,
      unusedHooks: this.results.unusedHooks.length,
      unusedServices: this.results.unusedServices.length,
      unusedTypes: this.results.unusedTypes.length,
      unusedComponents: this.results.unusedComponents.length,
      totalUnused: this.results.unusedHooks.length + 
                  this.results.unusedServices.length + 
                  this.results.unusedTypes.length + 
                  this.results.unusedComponents.length
    };
  }

  // إنتاج التقرير
  generateReport() {
    console.log('\n📊 تقرير تحليل الكود غير المستخدم');
    console.log('=' .repeat(50));
    
    console.log(`\n📈 الملخص:`);
    console.log(`   إجمالي الملفات: ${this.results.summary.totalFiles}`);
    console.log(`   Hooks غير مستخدمة: ${this.results.summary.unusedHooks}`);
    console.log(`   خدمات غير مستخدمة: ${this.results.summary.unusedServices}`);
    console.log(`   أنواع غير مستخدمة: ${this.results.summary.unusedTypes}`);
    console.log(`   مكونات غير مستخدمة: ${this.results.summary.unusedComponents}`);
    console.log(`   إجمالي غير المستخدم: ${this.results.summary.totalUnused}`);

    if (this.results.unusedHooks.length > 0) {
      console.log('\n🎣 Hooks غير مستخدمة:');
      this.results.unusedHooks.forEach(hook => {
        console.log(`   ❌ ${hook.file}`);
        console.log(`      السبب: ${hook.reason}`);
      });
    }

    if (this.results.unusedServices.length > 0) {
      console.log('\n🛠️  خدمات غير مستخدمة:');
      this.results.unusedServices.forEach(service => {
        console.log(`   ❌ ${service.file}`);
        console.log(`      السبب: ${service.reason}`);
        
        if (service.unusedExportedFunctions && service.unusedExportedFunctions.length > 0) {
          console.log(`      🔸 دوال مُصدَّرة غير مستخدمة:`);
          service.unusedExportedFunctions.forEach(func => {
            console.log(`         • ${func.name} (السطر: ${func.line})`);
          });
        }
        
        if (service.unusedInternalFunctions && service.unusedInternalFunctions.length > 0) {
          console.log(`      🔹 دوال داخلية غير مستخدمة:`);
          service.unusedInternalFunctions.forEach(func => {
            console.log(`         • ${func.name} (السطر: ${func.line})`);
          });
        }
      });
    }

    if (this.results.unusedTypes.length > 0) {
      console.log('\n📝 أنواع غير مستخدمة:');
      this.results.unusedTypes.forEach(type => {
        console.log(`   ❌ ${type.file}:${type.line}`);
        console.log(`      النوع: ${type.type}`);
        console.log(`      السبب: ${type.reason}`);
      });
    }

    if (this.results.unusedComponents.length > 0) {
      console.log('\n⚛️  مكونات غير مستخدمة:');
      this.results.unusedComponents.forEach(component => {
        console.log(`   ❌ ${component.file}`);
        console.log(`      السبب: ${component.reason}`);
      });
    }

    // حفظ التقرير في ملف JSON
    this.saveReportToFile();
    
    console.log('\n✅ تم الانتهاء من التحليل!');
    console.log(`📄 تم حفظ التقرير المفصل في: ${this.options.outputFile || 'unused-code-report.json'}`);
  }

  // حفظ التقرير في ملف
  saveReportToFile() {
    const fileName = this.options.outputFile || 'unused-code-report.json';
    const reportPath = path.join(this.projectRoot, fileName);
    
    try {
      fs.writeFileSync(reportPath, JSON.stringify(this.results, null, 2), 'utf-8');
      
      if (this.options.verbose) {
        console.log(`✅ تم حفظ التقرير في: ${reportPath}`);
      }
    } catch (error) {
      console.error(`❌ خطأ في حفظ التقرير: ${error.message}`);
    }
  }
}

// التحقق من وجود مجلد المشروع
function validateProjectPath(projectPath) {
  if (!fs.existsSync(projectPath)) {
    console.error(`❌ المسار غير موجود: ${projectPath}`);
    process.exit(1);
  }
  
  const packageJsonPath = path.join(projectPath, 'package.json');
  if (!fs.existsSync(packageJsonPath)) {
    console.warn(`⚠️  تحذير: لم يتم العثور على package.json في: ${projectPath}`);
    console.warn(`   هل أنت متأكد أن هذا مجلد مشروع؟`);
  }
}

// تشغيل المحلل
const { projectPath, options } = parseArguments();

console.log(`🚀 بدء تحليل المشروع في: ${path.resolve(projectPath)}`);

// التحقق من صحة المسار
validateProjectPath(projectPath);

const analyzer = new UnusedCodeAnalyzer(projectPath, options);
analyzer.analyze();

export default UnusedCodeAnalyzer;