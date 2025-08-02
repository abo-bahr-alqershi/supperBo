import React from 'react';
import {
  Box,
  Typography,
  Button,
  Link,
  Divider,
  useTheme,
  useMediaQuery
} from '@mui/material';
import {
  FormatQuote as QuoteIcon,
  ArrowForward as ArrowIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';
import DOMPurify from 'dompurify';

interface TextBlockProps {
  title?: string;
  subtitle?: string;
  content?: string;
  contentHtml?: string;
  textAlign?: 'left' | 'center' | 'right' | 'justify';
  variant?: 'default' | 'hero' | 'quote' | 'highlight' | 'minimal';
  titleVariant?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6';
  showDivider?: boolean;
  backgroundColor?: string;
  textColor?: string;
  accentColor?: string;
  padding?: number;
  maxWidth?: number | string;
  buttonText?: string;
  buttonUrl?: string;
  buttonVariant?: 'text' | 'outlined' | 'contained';
  author?: string;
  authorTitle?: string;
  isPreview?: boolean;
  onButtonClick?: (url: string) => void;
}

const TextBlock: React.FC<TextBlockProps> = ({
  title = 'Welcome to Our Platform',
  subtitle = 'Discover amazing features and services',
  content = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.',
  contentHtml,
  textAlign = 'left',
  variant = 'default',
  titleVariant = 'h4',
  showDivider = false,
  backgroundColor = 'transparent',
  textColor,
  accentColor,
  padding = 3,
  maxWidth = 'none',
  buttonText,
  buttonUrl,
  buttonVariant = 'contained',
  author,
  authorTitle,
  isPreview = false,
  onButtonClick
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));

  const handleButtonClick = () => {
    if (isPreview) return;
    
    if (onButtonClick && buttonUrl) {
      onButtonClick(buttonUrl);
    } else if (buttonUrl) {
      window.location.href = buttonUrl;
    }
  };

  const renderContent = () => {
    switch (variant) {
      case 'hero':
        return (
          <Box
            sx={{
              textAlign: 'center',
              py: { xs: 6, md: 10 },
              px: padding,
              backgroundColor,
              color: textColor
            }}
          >
            <motion.div
              initial={{ y: 30, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ duration: 0.5 }}
            >
              <Typography
                variant={isMobile ? 'h3' : 'h2'}
                component="h1"
                fontWeight="bold"
                gutterBottom
                sx={{ color: accentColor || 'primary.main' }}
              >
                {title}
              </Typography>
            </motion.div>

            <motion.div
              initial={{ y: 30, opacity: 0 }}
              animate={{ y: 0, opacity: 1 }}
              transition={{ duration: 0.5, delay: 0.1 }}
            >
              <Typography
                variant={isMobile ? 'h6' : 'h5'}
                color="text.secondary"
                paragraph
                sx={{ maxWidth: 800, mx: 'auto', mb: 4 }}
              >
                {subtitle}
              </Typography>
            </motion.div>

            {content && (
              <motion.div
                initial={{ y: 30, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ duration: 0.5, delay: 0.2 }}
              >
                <Typography
                  variant="body1"
                  paragraph
                  sx={{ maxWidth: 600, mx: 'auto', mb: 4 }}
                >
                  {content}
                </Typography>
              </motion.div>
            )}

            {buttonText && (
              <motion.div
                initial={{ y: 30, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ duration: 0.5, delay: 0.3 }}
              >
                <Button
                  variant={buttonVariant}
                  size="large"
                  onClick={handleButtonClick}
                  endIcon={<ArrowIcon />}
                  sx={{
                    backgroundColor: buttonVariant === 'contained' ? accentColor : undefined,
                    borderColor: buttonVariant === 'outlined' ? accentColor : undefined,
                    color: buttonVariant !== 'contained' ? accentColor : undefined
                  }}
                >
                  {buttonText}
                </Button>
              </motion.div>
            )}
          </Box>
        );

      case 'quote':
        return (
          <Box
            sx={{
              position: 'relative',
              p: padding,
              backgroundColor,
              borderLeft: `4px solid ${accentColor || theme.palette.primary.main}`,
              fontStyle: 'italic'
            }}
          >
            <QuoteIcon
              sx={{
                position: 'absolute',
                top: padding * 8,
                left: padding * 8,
                fontSize: 60,
                color: theme.palette.grey[200],
                transform: 'rotate(180deg)'
              }}
            />
            
            <Box sx={{ position: 'relative', zIndex: 1 }}>
              <Typography
                variant="h5"
                paragraph
                sx={{ color: textColor, fontStyle: 'italic' }}
              >
                "{content || contentHtml}"
              </Typography>
              
              {(author || authorTitle) && (
                <Box sx={{ mt: 2 }}>
                  {author && (
                    <Typography variant="body1" fontWeight="bold" color={accentColor}>
                      — {author}
                    </Typography>
                  )}
                  {authorTitle && (
                    <Typography variant="body2" color="text.secondary">
                      {authorTitle}
                    </Typography>
                  )}
                </Box>
              )}
            </Box>
          </Box>
        );

      case 'highlight':
        return (
          <Box
            sx={{
              p: padding,
              backgroundColor: backgroundColor || theme.palette.primary.light,
              borderRadius: 2,
              position: 'relative',
              overflow: 'hidden'
            }}
          >
            <Box
              sx={{
                position: 'absolute',
                top: -50,
                right: -50,
                width: 150,
                height: 150,
                borderRadius: '50%',
                backgroundColor: accentColor || theme.palette.primary.main,
                opacity: 0.1
              }}
            />
            
            {title && (
              <Typography
                variant={titleVariant}
                component="h2"
                gutterBottom
                fontWeight="bold"
                color={accentColor}
              >
                {title}
              </Typography>
            )}
            
            {subtitle && (
              <Typography
                variant="h6"
                color="text.secondary"
                paragraph
              >
                {subtitle}
              </Typography>
            )}
            
            {(content || contentHtml) && (
              <Typography
                variant="body1"
                paragraph
                sx={{ color: textColor }}
                dangerouslySetInnerHTML={contentHtml ? {
                  __html: DOMPurify.sanitize(contentHtml)
                } : undefined}
              >
                {!contentHtml && content}
              </Typography>
            )}
            
            {buttonText && (
              <Button
                variant={buttonVariant}
                onClick={handleButtonClick}
                sx={{
                  mt: 2,
                  backgroundColor: buttonVariant === 'contained' ? accentColor : undefined,
                  borderColor: buttonVariant === 'outlined' ? accentColor : undefined,
                  color: buttonVariant !== 'contained' ? accentColor : undefined
                }}
              >
                {buttonText}
              </Button>
            )}
          </Box>
        );

      case 'minimal':
        return (
          <Box sx={{ p: padding, textAlign }}>
            {title && (
              <Typography
                variant={titleVariant}
                component="h2"
                gutterBottom
                sx={{ color: textColor }}
              >
                {title}
              </Typography>
            )}
            
            {showDivider && title && (content || contentHtml) && (
              <Divider sx={{ my: 2, width: textAlign === 'center' ? 60 : '100%', mx: textAlign === 'center' ? 'auto' : 0 }} />
            )}
            
            {(content || contentHtml) && (
              <Typography
                variant="body1"
                sx={{ color: textColor }}
                dangerouslySetInnerHTML={contentHtml ? {
                  __html: DOMPurify.sanitize(contentHtml)
                } : undefined}
              >
                {!contentHtml && content}
              </Typography>
            )}
          </Box>
        );

      default:
        return (
          <Box
            sx={{
              p: padding,
              backgroundColor,
              textAlign,
              maxWidth: maxWidth === 'none' ? undefined : maxWidth,
              mx: maxWidth === 'none' ? 0 : 'auto'
            }}
          >
            {title && (
              <Typography
                variant={titleVariant}
                component="h2"
                gutterBottom
                fontWeight="bold"
                sx={{ color: textColor || accentColor }}
              >
                {title}
              </Typography>
            )}
            
            {subtitle && (
              <Typography
                variant="h6"
                color="text.secondary"
                paragraph
              >
                {subtitle}
              </Typography>
            )}
            
            {showDivider && (title || subtitle) && (content || contentHtml) && (
              <Divider sx={{ my: 3 }} />
            )}
            
            {(content || contentHtml) && (
              <Typography
                variant="body1"
                paragraph
                sx={{ 
                  color: textColor,
                  lineHeight: 1.8,
                  '& a': {
                    color: accentColor || theme.palette.primary.main,
                    textDecoration: 'none',
                    '&:hover': {
                      textDecoration: 'underline'
                    }
                  }
                }}
                dangerouslySetInnerHTML={contentHtml ? {
                  __html: DOMPurify.sanitize(contentHtml)
                } : undefined}
              >
                {!contentHtml && content}
              </Typography>
            )}
            
            {buttonText && (
              <Box sx={{ mt: 3, textAlign: textAlign === 'justify' ? 'left' : textAlign }}>
                <Button
                  variant={buttonVariant}
                  onClick={handleButtonClick}
                  endIcon={<ArrowIcon />}
                  sx={{
                    backgroundColor: buttonVariant === 'contained' ? accentColor : undefined,
                    borderColor: buttonVariant === 'outlined' ? accentColor : undefined,
                    color: buttonVariant !== 'contained' ? accentColor : undefined
                  }}
                >
                  {buttonText}
                </Button>
              </Box>
            )}
          </Box>
        );
    }
  };

  return renderContent();
};

export default TextBlock;