
// ===== خدمة API الشات =====

import type {
  ChatConversation,
  ChatMessage,
  ChatUser,
  CreateConversationRequest,
  SendMessageRequest,
  UpdateMessageStatusRequest,
  AddReactionRequest,
  SearchChatsRequest,
  GetConversationsRequest,
  GetMessagesRequest,
  ConversationsResponse,
  MessagesResponse,
  SearchResponse,
  FileUploadResponse,
  ChatEvent,
  ChatSettings,
  ChatError,
  ChatEventType,
  UserType,
  MessageType,
  ReactionType,
  MessageStatus,
  TypingIndicator,
  ConnectionStatus
} from '../types/chat.types';

/**
 * خدمة التواصل مع API الشات
 * تتولى جميع عمليات الاتصال مع الخادم
 */
export class ChatApiService {
  private baseUrl: string;
  private authToken: string | null = null;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  /**
   * تعيين رمز المصادقة
   */
  setAuthToken(token: string): void {
    this.authToken = token;
  }

  /**
   * إنشاء headers للطلبات
   */
  private getHeaders(): HeadersInit {
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    };

    if (this.authToken) {
      headers['Authorization'] = `Bearer ${this.authToken}`;
    }

    return headers;
  }

  /**
   * إنشاء headers للرفع
   */
  private getUploadHeaders(): HeadersInit {
    const headers: HeadersInit = {};

    if (this.authToken) {
      headers['Authorization'] = `Bearer ${this.authToken}`;
    }

    return headers;
  }

  /**
   * معالجة الاستجابة من الخادم
   */
  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const error = await response.json().catch(() => ({
        message: 'حدث خطأ في الشبكة'
      }));
      throw new Error(error.message || `خطأ HTTP: ${response.status}`);
    }
    return response.json();
  }

  // ===== إدارة المحادثات =====
  /**
   * الحصول على قائمة المحادثات
   */
  async getConversations(request: GetConversationsRequest): Promise<ConversationsResponse> {
    const params = new URLSearchParams();
    if (request.user_type) params.append('user_type', String(request.user_type));
    if (request.property_id) params.append('property_id', request.property_id);
    if (request.is_archived !== undefined) params.append('is_archived', String(request.is_archived));
    if (request.page) params.append('page', String(request.page));
    if (request.limit) params.append('limit', String(request.limit));

    const response = await fetch(`${this.baseUrl}/conversations?${params}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<ConversationsResponse>(response);
  }

  /**
   * إنشاء محادثة جديدة
   */
  async createConversation(request: CreateConversationRequest): Promise<ChatConversation> {
    const response = await fetch(`${this.baseUrl}/conversations`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(request),
    });

    return this.handleResponse<ChatConversation>(response);
  }

  /**
   * الحصول على تفاصيل محادثة
   */
  async getConversation(conversationId: string): Promise<ChatConversation> {
    const response = await fetch(`${this.baseUrl}/conversations/${conversationId}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<ChatConversation>(response);
  }

  /**
   * حذف محادثة
   */
  async deleteConversation(conversationId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/conversations/${conversationId}`, {
      method: 'DELETE',
      headers: this.getHeaders(),
    });

    await this.handleResponse<void>(response);
  }

  /**
   * أرشفة محادثة
   */
  async archiveConversation(conversationId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/conversations/${conversationId}/archive`, {
      method: 'POST',
      headers: this.getHeaders(),
    });

    await this.handleResponse<void>(response);
  }

  /**
   * إلغاء أرشفة محادثة
   */
  async unarchiveConversation(conversationId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/conversations/${conversationId}/unarchive`, {
      method: 'POST',
      headers: this.getHeaders(),
    });

    await this.handleResponse<void>(response);
  }

  // ===== إدارة الرسائل =====
  /**
   * الحصول على رسائل المحادثة
   */
  async getMessages(request: GetMessagesRequest): Promise<MessagesResponse> {
    const params = new URLSearchParams();
    if (request.page) params.append('page', String(request.page));
    if (request.limit) params.append('limit', String(request.limit));
    if (request.message_id_before) params.append('message_id_before', request.message_id_before);

    const response = await fetch(`${this.baseUrl}/conversations/${request.conversation_id}/messages?${params}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<MessagesResponse>(response);
  }

  /**
   * إرسال رسالة
   */
  async sendMessage(request: SendMessageRequest): Promise<ChatMessage> {
    const formData = new FormData();
    formData.append('message_type', String(request.message_type));
    
    if (request.content) {
      formData.append('content', request.content);
    }
    
    if (request.location) {
      formData.append('location', JSON.stringify(request.location));
    }
    
    if (request.reply_to_message_id) {
      formData.append('reply_to_message_id', request.reply_to_message_id);
    }

    if (request.attachments) {
      request.attachments.forEach((file, index) => {
        formData.append(`attachments[${index}]`, file);
      });
    }

    const response = await fetch(`${this.baseUrl}/conversations/${request.conversation_id}/messages`, {
      method: 'POST',
      headers: this.getUploadHeaders(),
      body: formData,
    });

    return this.handleResponse<ChatMessage>(response);
  }

  /**
   * تحديث حالة الرسالة
   */
  async updateMessageStatus(request: UpdateMessageStatusRequest): Promise<void> {
    const response = await fetch(`${this.baseUrl}/messages/${request.message_id}/status`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify({
        status: request.status,
        read_by: request.read_by
      }),
    });

    await this.handleResponse<void>(response);
  }

  /**
   * حذف رسالة
   */
  async deleteMessage(messageId: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/messages/${messageId}`, {
      method: 'DELETE',
      headers: this.getHeaders(),
    });

    await this.handleResponse<void>(response);
  }

  /**
   * تعديل رسالة
   */
  async editMessage(messageId: string, newContent: string): Promise<ChatMessage> {
    const response = await fetch(`${this.baseUrl}/messages/${messageId}`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify({ content: newContent }),
    });

    return this.handleResponse<ChatMessage>(response);
  }

  // ===== إدارة التفاعلات =====
  /**
   * إضافة تفاعل للرسالة
   */
  async addReaction(request: AddReactionRequest): Promise<void> {
    const response = await fetch(`${this.baseUrl}/messages/${request.message_id}/reactions`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify({ reaction_type: request.reaction_type }),
    });

    await this.handleResponse<void>(response);
  }

  /**
   * إزالة تفاعل من الرسالة
   */
  async removeReaction(messageId: string, reactionType: ReactionType): Promise<void> {
    const response = await fetch(`${this.baseUrl}/messages/${messageId}/reactions/${reactionType}`, {
      method: 'DELETE',
      headers: this.getHeaders(),
    });

    await this.handleResponse<void>(response);
  }

  // ===== البحث =====
  /**
   * البحث في المحادثات والرسائل
   */
  async searchChats(request: SearchChatsRequest): Promise<SearchResponse> {
    const params = new URLSearchParams();
    params.append('query', request.query);
    if (request.conversation_id) params.append('conversation_id', request.conversation_id);
    if (request.message_type) params.append('message_type', String(request.message_type));
    if (request.sender_id) params.append('sender_id', request.sender_id);
    if (request.date_from) params.append('date_from', request.date_from.toISOString());
    if (request.date_to) params.append('date_to', request.date_to.toISOString());
    if (request.page) params.append('page', String(request.page));
    if (request.limit) params.append('limit', String(request.limit));

    const response = await fetch(`${this.baseUrl}/search?${params}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<SearchResponse>(response);
  }

  // ===== إدارة المستخدمين =====
  /**
   * الحصول على قائمة المستخدمين المتاحين للمحادثة
   */
  async getAvailableUsers(userType?: UserType, propertyId?: string): Promise<ChatUser[]> {
    const params = new URLSearchParams();
    if (userType) params.append('user_type', String(userType));
    if (propertyId) params.append('property_id', propertyId);

    const response = await fetch(`${this.baseUrl}/users/available?${params}`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<ChatUser[]>(response);
  }

  /**
   * تحديث حالة المستخدم
   */
  async updateUserStatus(status: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/users/status`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify({ status }),
    });

    await this.handleResponse<void>(response);
  }

  // ===== إدارة الإعدادات =====
  /**
   * الحصول على إعدادات الشات
   */
  async getChatSettings(): Promise<ChatSettings> {
    const response = await fetch(`${this.baseUrl}/settings`, {
      method: 'GET',
      headers: this.getHeaders(),
    });

    return this.handleResponse<ChatSettings>(response);
  }

  /**
   * تحديث إعدادات الشات
   */
  async updateChatSettings(settings: Partial<ChatSettings>): Promise<ChatSettings> {
    const response = await fetch(`${this.baseUrl}/settings`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify(settings),
    });

    return this.handleResponse<ChatSettings>(response);
  }

  // ===== رفع الملفات =====
  /**
   * رفع ملف
   */
  async uploadFile(file: File, messageType: MessageType): Promise<FileUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('message_type', String(messageType));

    const response = await fetch(`${this.baseUrl}/upload`, {
      method: 'POST',
      headers: this.getUploadHeaders(),
      body: formData,
    });

    return this.handleResponse<FileUploadResponse>(response);
  }
}

// ===== خدمة FCM للإشعارات =====
/**
 * خدمة إدارة إشعارات FCM
 */
export class FCMService {
  private vapidKey: string;
  private messaging: any = null;

  constructor(vapidKey: string) {
    this.vapidKey = vapidKey;
  }

  /**
   * تهيئة خدمة FCM
   */
  async initialize(): Promise<void> {
    try {
      // تحقق من دعم الإشعارات
      if (!('Notification' in window)) {
        throw new Error('هذا المتصفح لا يدعم الإشعارات');
      }

      // طلب إذن الإشعارات
      const permission = await Notification.requestPermission();
      if (permission !== 'granted') {
        throw new Error('تم رفض إذن الإشعارات');
      }

      // تهيئة Firebase Messaging
      const { initializeApp } = await import('firebase/app');
      const { getMessaging, getToken, onMessage } = await import('firebase/messaging');

      const firebaseConfig = {
        // إعدادات Firebase - يجب تحديثها
        apiKey: "your-api-key",
        authDomain: "your-auth-domain",
        projectId: "your-project-id",
        storageBucket: "your-storage-bucket",
        messagingSenderId: "your-messaging-sender-id",
        appId: "your-app-id"
      };

      const app = initializeApp(firebaseConfig);
      this.messaging = getMessaging(app);

      // الاستماع للرسائل الواردة
      onMessage(this.messaging, (payload) => {
        console.log('تم استلام رسالة FCM:', payload);
        this.handleIncomingMessage(payload);
      });

    } catch (error) {
      console.error('خطأ في تهيئة FCM:', error);
      throw error;
    }
  }

  /**
   * الحصول على رمز FCM
   */
  async getToken(): Promise<string> {
    if (!this.messaging) {
      throw new Error('لم يتم تهيئة FCM بعد');
    }

    try {
      const { getToken } = await import('firebase/messaging');
      const token = await getToken(this.messaging, {
        vapidKey: this.vapidKey
      });

      if (!token) {
        throw new Error('فشل في الحصول على رمز FCM');
      }

      return token;
    } catch (error) {
      console.error('خطأ في الحصول على رمز FCM:', error);
      throw error;
    }
  }

  /**
   * معالجة الرسائل الواردة
   */
  private handleIncomingMessage(payload: any): void {
    const { notification, data } = payload;
    
    // إنشاء إشعار مخصص
    if (notification) {
      this.showNotification(notification.title, {
        body: notification.body,
        icon: notification.icon || '/icons/chat-icon.png',
        badge: '/icons/badge-icon.png',
        data: data,
        tag: data?.conversation_id || 'chat-notification',
        requireInteraction: true
      });
    }

    // إرسال الحدث للتطبيق
    if (data) {
      window.dispatchEvent(new CustomEvent('fcm-message', {
        detail: {
          type: data.type,
          payload: data
        }
      }));
    }
  }

  /**
   * عرض إشعار
   */
  private showNotification(title: string, options: NotificationOptions): void {
    if ('serviceWorker' in navigator) {
      navigator.serviceWorker.ready.then(registration => {
        registration.showNotification(title, options);
      });
    } else {
      new Notification(title, options);
    }
  }

  /**
   * إرسال رمز FCM للخادم
   */
  async registerToken(token: string, userId: string): Promise<void> {
    try {
      const response = await fetch('/api/fcm/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          token,
          user_id: userId,
          device_type: 'web'
        }),
      });

      if (!response.ok) {
        throw new Error('فشل في تسجيل رمز FCM');
      }
    } catch (error) {
      console.error('خطأ في تسجيل رمز FCM:', error);
      throw error;
    }
  }
}

// ===== خدمة WebSocket للشات المباشر =====
/**
 * خدمة WebSocket لإدارة الاتصال المباشر
 */
export class ChatWebSocketService {
  private socket: WebSocket | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;
  private reconnectDelay = 1000;
  private eventHandlers: Map<string, Function[]> = new Map();
  private isConnected = false;
  private userId: string | null = null;
  private authToken: string | null = null;
  private wsUrl: string;

  constructor(wsUrl: string) {
    this.wsUrl = wsUrl;
  }

  /**
   * الاتصال بالخادم
   */
  connect(userId: string, authToken: string): Promise<void> {
    return new Promise((resolve, reject) => {
      this.userId = userId;
      this.authToken = authToken;

      try {
        // إنشاء اتصال WebSocket
        const wsUrlWithAuth = `${this.wsUrl}?token=${authToken}&user_id=${userId}`;
        this.socket = new WebSocket(wsUrlWithAuth);

        // معالجة الأحداث
        this.socket.onopen = () => {
          console.log('تم الاتصال بخادم الشات');
          this.isConnected = true;
          this.reconnectAttempts = 0;
          this.emit('connected');
          resolve();
        };

        this.socket.onmessage = (event) => {
          try {
            const data = JSON.parse(event.data);
            this.handleMessage(data);
          } catch (error) {
            console.error('خطأ في معالجة رسالة WebSocket:', error);
          }
        };

        this.socket.onclose = (event) => {
          console.log('انقطع الاتصال بخادم الشات');
          this.isConnected = false;
          this.emit('disconnected');
          
          // إعادة المحاولة في حالة انقطاع غير متوقع
          if (!event.wasClean && this.reconnectAttempts < this.maxReconnectAttempts) {
            this.attemptReconnect();
          }
        };

        this.socket.onerror = (error) => {
          console.error('خطأ في WebSocket:', error);
          this.emit('error', error);
          reject(error);
        };

      } catch (error) {
        console.error('خطأ في إنشاء اتصال WebSocket:', error);
        reject(error);
      }
    });
  }

  /**
   * قطع الاتصال
   */
  disconnect(): void {
    if (this.socket) {
      this.socket.close(1000, 'قطع الاتصال بواسطة المستخدم');
      this.socket = null;
      this.isConnected = false;
    }
  }

  /**
   * إرسال رسالة عبر WebSocket
   */
  sendMessage(type: string, data: any): void {
    if (!this.isConnected || !this.socket) {
      console.warn('لا يمكن إرسال الرسالة - الاتصال غير متوفر');
      return;
    }

    try {
      const message = {
        type,
        data,
        timestamp: new Date().toISOString()
      };

      this.socket.send(JSON.stringify(message));
    } catch (error) {
      console.error('خطأ في إرسال رسالة WebSocket:', error);
    }
  }

  /**
   * معالجة الرسائل الواردة
   */
  private handleMessage(message: any): void {
    // New envelope: { event_type, data }
    const { event_type, data } = message;
    const type = event_type;
    
    switch (type) {
      case 'new_message':
        // data: { conversation_id, message }
        this.emit('new_message', data.message);
        break;
      case 'message_updated':
        // data: { conversation_id, message }
        this.emit('message_updated', data.message);
        break;
      case 'message_deleted':
        // data: { conversation_id, message_id }
        this.emit('message_deleted', data);
        break;
      case 'typing_indicator':
        // data: TypingIndicator
        this.emit('typing_indicator', data);
        break;
      case 'user_status_changed':
        // data: { user_id, status, timestamp }
        this.emit('user_status_changed', data);
        break;
      case 'conversation_created':
        // data: { conversation }
        this.emit('conversation_created', data.conversation);
        break;
      case 'reaction_added':
        // data: { message_id, reaction }
        this.emit('reaction_added', { message_id: data.message_id, reaction: data.reaction });
        break;
      case 'reaction_removed':
        // data: { message_id, reaction_id }
        this.emit('reaction_removed', { message_id: data.message_id, reaction_id: data.reaction_id });
        break;
      default:
        console.log('نوع رسالة غير معروف:', type);
    }
  }

  /**
   * إضافة مستمع للأحداث
   */
  on(event: string, handler: Function): void {
    if (!this.eventHandlers.has(event)) {
      this.eventHandlers.set(event, []);
    }
    this.eventHandlers.get(event)!.push(handler);
  }

  /**
   * إزالة مستمع الأحداث
   */
  off(event: string, handler: Function): void {
    const handlers = this.eventHandlers.get(event);
    if (handlers) {
      const index = handlers.indexOf(handler);
      if (index > -1) {
        handlers.splice(index, 1);
      }
    }
  }

  /**
   * إرسال حدث
   */
  private emit(event: string, data?: any): void {
    const handlers = this.eventHandlers.get(event);
    if (handlers) {
      handlers.forEach(handler => {
        try {
          handler(data);
        } catch (error) {
          console.error('خطأ في معالج الحدث:', error);
        }
      });
    }
  }

  /**
   * محاولة إعادة الاتصال
   */
  private attemptReconnect(): void {
    if (this.reconnectAttempts >= this.maxReconnectAttempts) {
      console.error('تم الوصول للحد الأقصى من محاولات إعادة الاتصال');
      return;
    }

    this.reconnectAttempts++;
    const delay = this.reconnectDelay * Math.pow(2, this.reconnectAttempts - 1);

    console.log(`محاولة إعادة الاتصال (${this.reconnectAttempts}/${this.maxReconnectAttempts}) خلال ${delay}ms`);

    setTimeout(() => {
      if (this.userId && this.authToken) {
        this.connect(this.userId, this.authToken).catch(error => {
          console.error('فشل في إعادة الاتصال:', error);
        });
      }
    }, delay);
  }

  /**
   * إرسال مؤشر الكتابة
   */
  sendTypingIndicator(conversationId: string, isTyping: boolean): void {
    this.sendMessage('typing_indicator', {
      conversation_id: conversationId,
      is_typing: isTyping
    });
  }

  /**
   * تحديث حالة المستخدم
   */
  updateUserStatus(status: string): void {
    this.sendMessage('user_status_update', {
      status
    });
  }

  /**
   * الانضمام لمحادثة
   */
  joinConversation(conversationId: string): void {
    this.sendMessage('join_conversation', {
      conversation_id: conversationId
    });
  }

  /**
   * مغادرة محادثة
   */
  leaveConversation(conversationId: string): void {
    this.sendMessage('leave_conversation', {
      conversation_id: conversationId
    });
  }

  /**
   * التحقق من حالة الاتصال
   */
  isConnectedToServer(): boolean {
    return this.isConnected && this.socket?.readyState === WebSocket.OPEN;
  }
}

// ===== خدمة إدارة الشات الرئيسية =====
/**
 * خدمة إدارة الشات الرئيسية
 * تجمع جميع الخدمات وتوفر واجهة موحدة
 */
export class ChatService {
  private apiService: ChatApiService;
  private fcmService: FCMService;
  private wsService: ChatWebSocketService;
  private currentUser: ChatUser | null = null;

  constructor(
    apiBaseUrl: string,
    wsUrl: string,
    fcmVapidKey: string
  ) {
    this.apiService = new ChatApiService(apiBaseUrl);
    this.fcmService = new FCMService(fcmVapidKey);
    this.wsService = new ChatWebSocketService(wsUrl);
  }

  /**
   * تهيئة خدمة الشات
   */
  async initialize(authToken: string, user: ChatUser): Promise<void> {
    try {
      this.currentUser = user;
      this.apiService.setAuthToken(authToken);

      // تهيئة FCM
      await this.fcmService.initialize();
      const fcmToken = await this.fcmService.getToken();
      await this.fcmService.registerToken(fcmToken, user.user_id);

      // تهيئة WebSocket
      await this.wsService.connect(user.user_id, authToken);

      console.log('تم تهيئة خدمة الشات بنجاح');
    } catch (error) {
      console.error('خطأ في تهيئة خدمة الشات:', error);
      throw error;
    }
  }

  /**
   * إنهاء خدمة الشات
   */
  cleanup(): void {
    this.wsService.disconnect();
    this.currentUser = null;
  }

  // ===== دالات الوصول للخدمات =====
  get api(): ChatApiService {
    return this.apiService;
  }

  get fcm(): FCMService {
    return this.fcmService;
  }

  get ws(): ChatWebSocketService {
    return this.wsService;
  }

  get user(): ChatUser | null {
    return this.currentUser;
  }

  // ===== دالات مساعدة =====
  /**
   * التحقق من إمكانية التواصل مع مستخدم
   */
  canCommunicateWith(targetUser: ChatUser): boolean {
    if (!this.currentUser) {
      return false;
    }

    const currentUserType = this.currentUser.user_type;
    const targetUserType = targetUser.user_type;

    // الإدارة يمكنها التواصل مع الجميع
    if (currentUserType === 'admin') {
      return true;
    }

    // العملاء يمكنهم التواصل مع الإدارة والفنادق
    if (currentUserType === 'customer') {
      return targetUserType === 'admin' || targetUserType === 'property_owner';
    }

    // الفنادق يمكنها التواصل مع الإدارة
    if (currentUserType === 'property_owner' && targetUserType === 'admin') {
      return true;
    }

    // الفنادق يمكنها التواصل مع العملاء فقط إذا كان لديهم تفاعل سابق
    // (هذا سيتم التحقق منه في الخادم)
    if (currentUserType === 'property_owner' && targetUserType === 'customer') {
      return true; // التحقق التفصيلي في الخادم
    }

    return false;
  }

  /**
   * تنسيق حجم الملف
   */
  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  /**
   * التحقق من نوع الملف المسموح
   */
  isFileTypeAllowed(file: File, messageType: MessageType): boolean {
    const allowedTypes: Record<string, string[]> = {
      'image': ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
      'video': ['video/mp4', 'video/webm', 'video/ogg'],
      'audio': ['audio/mp3', 'audio/wav', 'audio/ogg', 'audio/m4a'],
      'document': ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
    };

    return allowedTypes[messageType as string]?.includes(file.type) || false;
  }

  /**
   * الحد الأقصى لحجم الملف
   */
  getMaxFileSize(messageType: MessageType): number {
    const maxSizes: Record<string, number> = {
      'image': 5 * 1024 * 1024, // 5MB
      'video': 50 * 1024 * 1024, // 50MB
      'audio': 10 * 1024 * 1024, // 10MB
      'document': 10 * 1024 * 1024 // 10MB
    };

    return maxSizes[messageType as string] || 1024 * 1024; // 1MB افتراضي
  }
}