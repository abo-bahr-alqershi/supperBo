import React, { useState, useRef, useEffect } from 'react';
import {
  Box,
  TextField,
  InputAdornment,
  IconButton,
  Button,
  Paper,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Typography,
  Chip,
  Divider,
  CircularProgress,
  useTheme,
  useMediaQuery,
  Popper,
  Fade
} from '@mui/material';
import {
  Search as SearchIcon,
  Clear as ClearIcon,
  FilterList as FilterIcon,
  LocationOn as LocationIcon,
  History as HistoryIcon,
  TrendingUp as TrendingIcon,
  Mic as MicIcon,
  CameraAlt as CameraIcon
} from '@mui/icons-material';
import { motion } from 'framer-motion';

interface SearchSuggestion {
  id: string;
  text: string;
  type: 'recent' | 'trending' | 'location';
  icon?: React.ReactNode;
}

interface SearchBarProps {
  placeholder?: string;
  variant?: 'standard' | 'outlined' | 'filled';
  size?: 'small' | 'medium' | 'large';
  showFilters?: boolean;
  showVoiceSearch?: boolean;
  showImageSearch?: boolean;
  showSuggestions?: boolean;
  suggestions?: SearchSuggestion[];
  recentSearches?: string[];
  trendingSearches?: string[];
  autoFocus?: boolean;
  fullWidth?: boolean;
  elevation?: number;
  borderRadius?: number;
  backgroundColor?: string;
  isPreview?: boolean;
  onSearch?: (query: string) => void;
  onFilterClick?: () => void;
  onVoiceSearch?: () => void;
  onImageSearch?: () => void;
  onSuggestionClick?: (suggestion: SearchSuggestion) => void;
}

const SearchBar: React.FC<SearchBarProps> = ({
  placeholder = 'Search properties, locations...',
  variant = 'outlined',
  size = 'medium',
  showFilters = true,
  showVoiceSearch = true,
  showImageSearch = false,
  showSuggestions = true,
  suggestions = [],
  recentSearches = [],
  trendingSearches = [],
  autoFocus = false,
  fullWidth = true,
  elevation = 2,
  borderRadius = 8,
  backgroundColor = '#ffffff',
  isPreview = false,
  onSearch,
  onFilterClick,
  onVoiceSearch,
  onImageSearch,
  onSuggestionClick
}) => {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  const [query, setQuery] = useState('');
  const [isFocused, setIsFocused] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [showSuggestionsPanel, setShowSuggestionsPanel] = useState(false);
  const searchInputRef = useRef<HTMLInputElement>(null);
  const anchorRef = useRef<HTMLDivElement>(null);

  const allSuggestions = [
    ...recentSearches.map((text, index) => ({
      id: `recent-${index}`,
      text,
      type: 'recent' as const,
      icon: <HistoryIcon />
    })),
    ...trendingSearches.map((text, index) => ({
      id: `trending-${index}`,
      text,
      type: 'trending' as const,
      icon: <TrendingIcon />
    })),
    ...suggestions
  ];

  useEffect(() => {
    if (showSuggestions && query.length > 0 && isFocused) {
      setShowSuggestionsPanel(true);
    } else if (query.length === 0 && isFocused && allSuggestions.length > 0) {
      setShowSuggestionsPanel(true);
    } else {
      setShowSuggestionsPanel(false);
    }
  }, [query, isFocused, showSuggestions, allSuggestions.length]);

  const handleSearch = () => {
    if (isPreview || !query.trim()) return;
    
    setIsLoading(true);
    onSearch?.(query);
    
    // Simulate search completion
    setTimeout(() => {
      setIsLoading(false);
      setShowSuggestionsPanel(false);
    }, 500);
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  const handleClear = () => {
    setQuery('');
    searchInputRef.current?.focus();
  };

  const handleSuggestionClick = (suggestion: SearchSuggestion) => {
    if (isPreview) return;
    
    setQuery(suggestion.text);
    setShowSuggestionsPanel(false);
    onSuggestionClick?.(suggestion);
    handleSearch();
  };

  const getInputSize = () => {
    switch (size) {
      case 'small': return { height: 40, fontSize: 14 };
      case 'large': return { height: 56, fontSize: 18 };
      default: return { height: 48, fontSize: 16 };
    }
  };

  const inputSize = getInputSize();

  return (
    <Box
      ref={anchorRef}
      sx={{
        width: fullWidth ? '100%' : 'auto',
        position: 'relative'
      }}
    >
      <Paper
        elevation={isFocused ? elevation + 2 : elevation}
        sx={{
          display: 'flex',
          alignItems: 'center',
          backgroundColor,
          borderRadius: borderRadius / 8,
          transition: 'all 0.3s',
          overflow: 'hidden'
        }}
      >
        <TextField
          ref={searchInputRef}
          fullWidth
          variant={variant}
          placeholder={placeholder}
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          onFocus={() => setIsFocused(true)}
          onBlur={() => setTimeout(() => setIsFocused(false), 200)}
          onKeyPress={handleKeyPress}
          autoFocus={autoFocus}
          InputProps={{
            sx: {
              height: inputSize.height,
              fontSize: inputSize.fontSize,
              pr: 0
            },
            startAdornment: (
              <InputAdornment position="start">
                {isLoading ? (
                  <CircularProgress size={20} />
                ) : (
                  <SearchIcon />
                )}
              </InputAdornment>
            ),
            endAdornment: (
              <InputAdornment position="end">
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  {query && (
                    <IconButton
                      size="small"
                      onClick={handleClear}
                      sx={{ mr: 1 }}
                    >
                      <ClearIcon />
                    </IconButton>
                  )}
                  
                  {showVoiceSearch && !isMobile && (
                    <IconButton
                      size="small"
                      onClick={() => !isPreview && onVoiceSearch?.()}
                      sx={{ mr: 1 }}
                    >
                      <MicIcon />
                    </IconButton>
                  )}
                  
                  {showImageSearch && !isMobile && (
                    <IconButton
                      size="small"
                      onClick={() => !isPreview && onImageSearch?.()}
                      sx={{ mr: 1 }}
                    >
                      <CameraIcon />
                    </IconButton>
                  )}
                  
                  {showFilters && (
                    <Divider orientation="vertical" flexItem sx={{ mx: 1 }} />
                  )}
                </Box>
              </InputAdornment>
            )
          }}
        />
        
        {showFilters && (
          <Button
            variant="text"
            startIcon={<FilterIcon />}
            onClick={() => !isPreview && onFilterClick?.()}
            sx={{
              height: '100%',
              borderRadius: 0,
              px: 2,
              minWidth: isMobile ? 'auto' : 120
            }}
          >
            {!isMobile && 'Filters'}
          </Button>
        )}
      </Paper>

      {/* Suggestions Panel */}
      <Popper
        open={showSuggestionsPanel && !isPreview}
        anchorEl={anchorRef.current}
        placement="bottom-start"
        style={{ width: anchorRef.current?.offsetWidth, zIndex: 1300 }}
        transition
      >
        {({ TransitionProps }) => (
          <Fade {...TransitionProps} timeout={200}>
            <Paper
              elevation={8}
              sx={{
                mt: 1,
                maxHeight: 400,
                overflow: 'auto',
                borderRadius: borderRadius / 8
              }}
            >
              {query.length === 0 && allSuggestions.length > 0 && (
                <>
                  {recentSearches.length > 0 && (
                    <>
                      <Box sx={{ p: 2, pb: 1 }}>
                        <Typography variant="caption" color="text.secondary">
                          Recent Searches
                        </Typography>
                      </Box>
                      <List dense>
                        {allSuggestions
                          .filter(s => s.type === 'recent')
                          .slice(0, 3)
                          .map((suggestion) => (
                            <ListItem
                              key={suggestion.id}
                              button
                              onClick={() => handleSuggestionClick(suggestion)}
                            >
                              <ListItemIcon>
                                {suggestion.icon}
                              </ListItemIcon>
                              <ListItemText primary={suggestion.text} />
                            </ListItem>
                          ))}
                      </List>
                      <Divider />
                    </>
                  )}
                  
                  {trendingSearches.length > 0 && (
                    <>
                      <Box sx={{ p: 2, pb: 1 }}>
                        <Typography variant="caption" color="text.secondary">
                          Trending
                        </Typography>
                      </Box>
                      <List dense>
                        {allSuggestions
                          .filter(s => s.type === 'trending')
                          .slice(0, 5)
                          .map((suggestion) => (
                            <ListItem
                              key={suggestion.id}
                              button
                              onClick={() => handleSuggestionClick(suggestion)}
                            >
                              <ListItemIcon>
                                {suggestion.icon}
                              </ListItemIcon>
                              <ListItemText primary={suggestion.text} />
                            </ListItem>
                          ))}
                      </List>
                    </>
                  )}
                </>
              )}
              
              {query.length > 0 && (
                <List dense>
                  {allSuggestions
                    .filter(s => s.text.toLowerCase().includes(query.toLowerCase()))
                    .slice(0, 8)
                    .map((suggestion) => (
                      <ListItem
                        key={suggestion.id}
                        button
                        onClick={() => handleSuggestionClick(suggestion)}
                      >
                        <ListItemIcon>
                          {suggestion.icon || <SearchIcon />}
                        </ListItemIcon>
                        <ListItemText
                          primary={
                            <Typography>
                              {suggestion.text.split(new RegExp(`(${query})`, 'gi')).map((part, i) =>
                                part.toLowerCase() === query.toLowerCase() ? (
                                  <strong key={i}>{part}</strong>
                                ) : (
                                  part
                                )
                              )}
                            </Typography>
                          }
                        />
                      </ListItem>
                    ))}
                </List>
              )}
            </Paper>
          </Fade>
        )}
      </Popper>
    </Box>
  );
};

export default SearchBar;