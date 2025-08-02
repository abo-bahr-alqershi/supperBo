import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Box,
  Button,
  IconButton,
  Typography,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Card,
  CardContent,
  CardActions,
  Grid,
  Chip,
  Divider,
  Tab,
  Tabs,
  Alert,
  CircularProgress,
  InputAdornment,
  Menu,
  ListItemIcon,
  ListItemText
} from '@mui/material';
import {
  Close as CloseIcon,
  Add as AddIcon,
  Search as SearchIcon,
  Edit as EditIcon,
  FileCopy as CopyIcon,
  Delete as DeleteIcon,
  Publish as PublishIcon,
  MoreVert as MoreIcon,
  Check as CheckIcon
} from '@mui/icons-material';
import { toast } from 'react-hot-toast';
import homeScreenService from '../../services/homeScreenService';
import type { 
  HomeScreenTemplate, 
  CreateHomeScreenTemplateCommand,
  Platform,
  TargetAudience 
} from '../../types/homeScreen.types';

interface TemplateManagerProps {
  open?: boolean;
  onClose: () => void;
  currentTemplateId?: string;
}

interface CreateTemplateDialogProps {
  open: boolean;
  onClose: () => void;
  onSuccess: (template: HomeScreenTemplate) => void;
}

const CreateTemplateDialog: React.FC<CreateTemplateDialogProps> = ({
  open,
  onClose,
  onSuccess
}) => {
  const [formData, setFormData] = useState<CreateHomeScreenTemplateCommand>({
    name: '',
    description: '',
    version: '1.0.0',
    platform: 'All',
    targetAudience: 'All',
    metaData: '{}'
  });

  const createMutation = useMutation({
    mutationFn: (command: CreateHomeScreenTemplateCommand) =>
      homeScreenService.createTemplate(command),
    onSuccess: (data) => {
      toast.success('Template created successfully');
      onSuccess(data);
      onClose();
    },
    onError: () => {
      toast.error('Failed to create template');
    }
  });

  const handleSubmit = () => {
    if (!formData.name.trim()) {
      toast.error('Template name is required');
      return;
    }
    createMutation.mutate(formData);
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Create New Template</DialogTitle>
      <DialogContent>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
          <TextField
            fullWidth
            label="Template Name"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            required
          />
          
          <TextField
            fullWidth
            label="Description"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            multiline
            rows={3}
          />
          
          <TextField
            fullWidth
            label="Version"
            value={formData.version}
            onChange={(e) => setFormData({ ...formData, version: e.target.value })}
          />
          
          <FormControl fullWidth>
            <InputLabel>Platform</InputLabel>
            <Select
              value={formData.platform}
              onChange={(e) => setFormData({ ...formData, platform: e.target.value as Platform })}
            >
              <MenuItem value="All">All Platforms</MenuItem>
              <MenuItem value="iOS">iOS Only</MenuItem>
              <MenuItem value="Android">Android Only</MenuItem>
            </Select>
          </FormControl>
          
          <FormControl fullWidth>
            <InputLabel>Target Audience</InputLabel>
            <Select
              value={formData.targetAudience}
              onChange={(e) => setFormData({ ...formData, targetAudience: e.target.value as TargetAudience })}
            >
              <MenuItem value="All">All Users</MenuItem>
              <MenuItem value="Guest">Guest Users</MenuItem>
              <MenuItem value="User">Registered Users</MenuItem>
              <MenuItem value="Premium">Premium Users</MenuItem>
            </Select>
          </FormControl>
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button 
          onClick={handleSubmit} 
          variant="contained"
          disabled={createMutation.isPending}
        >
          {createMutation.isPending ? 'Creating...' : 'Create'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

const TemplateManager: React.FC<TemplateManagerProps> = ({
  open = true,
  onClose,
  currentTemplateId
}) => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [activeTab, setActiveTab] = useState(0);
  const [searchQuery, setSearchQuery] = useState('');
  const [showCreateDialog, setShowCreateDialog] = useState(false);
  const [selectedTemplate, setSelectedTemplate] = useState<HomeScreenTemplate | null>(null);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  // Fetch templates
  const { data: templates = [], isLoading } = useQuery({
    queryKey: ['homeScreenTemplates'],
    queryFn: () => homeScreenService.getTemplates()
  });

  // Delete mutation
  const deleteMutation = useMutation({
    mutationFn: (id: string) => homeScreenService.deleteTemplate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplates'] });
      toast.success('Template deleted successfully');
      setAnchorEl(null);
    },
    onError: () => {
      toast.error('Failed to delete template');
    }
  });

  // Duplicate mutation
  const duplicateMutation = useMutation({
    mutationFn: (id: string) => homeScreenService.duplicateTemplate(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplates'] });
      toast.success('Template duplicated successfully');
      setAnchorEl(null);
    },
    onError: () => {
      toast.error('Failed to duplicate template');
    }
  });

  // Publish mutation
  const publishMutation = useMutation({
    mutationFn: ({ id, deactivateOthers }: { id: string; deactivateOthers: boolean }) =>
      homeScreenService.publishTemplate(id, deactivateOthers),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['homeScreenTemplates'] });
      toast.success('Template published successfully');
      setAnchorEl(null);
    },
    onError: () => {
      toast.error('Failed to publish template');
    }
  });

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>, template: HomeScreenTemplate) => {
    setAnchorEl(event.currentTarget);
    setSelectedTemplate(template);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedTemplate(null);
  };

  const handleEditTemplate = (template: HomeScreenTemplate) => {
    navigate(`/home-screen-builder/${template.id}`);
    onClose();
  };

  const handleDeleteTemplate = () => {
    if (selectedTemplate && window.confirm('Are you sure you want to delete this template?')) {
      deleteMutation.mutate(selectedTemplate.id);
    }
  };

  const handleDuplicateTemplate = () => {
    if (selectedTemplate) {
      duplicateMutation.mutate(selectedTemplate.id);
    }
  };

  const handlePublishTemplate = () => {
    if (selectedTemplate) {
      const deactivateOthers = window.confirm(
        'Do you want to deactivate other templates? This will make this template the only active one.'
      );
      publishMutation.mutate({ id: selectedTemplate.id, deactivateOthers });
    }
  };

  const filteredTemplates = templates.filter(template =>
    template.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    template.description.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const activeTemplates = filteredTemplates.filter(t => t.isActive);
  const draftTemplates = filteredTemplates.filter(t => !t.isActive);

  return (
    <>
      <Dialog 
        open={open} 
        onClose={onClose} 
        maxWidth="lg" 
        fullWidth
        PaperProps={{
          sx: { height: '80vh' }
        }}
      >
        <DialogTitle>
          <Box display="flex" alignItems="center" justifyContent="space-between">
            <Typography variant="h6">Template Manager</Typography>
            <IconButton onClick={onClose}>
              <CloseIcon />
            </IconButton>
          </Box>
        </DialogTitle>

        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={activeTab} onChange={(_, value) => setActiveTab(value)}>
            <Tab label="All Templates" />
            <Tab label="Active" />
            <Tab label="Drafts" />
          </Tabs>
        </Box>

        <DialogContent>
          <Box sx={{ mb: 3, display: 'flex', gap: 2 }}>
            <TextField
              fullWidth
              placeholder="Search templates..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                )
              }}
            />
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => setShowCreateDialog(true)}
              sx={{ minWidth: 150 }}
            >
              New Template
            </Button>
          </Box>

          {isLoading ? (
            <Box display="flex" justifyContent="center" py={4}>
              <CircularProgress />
            </Box>
          ) : (
            <Grid container spacing={2}>
              {(activeTab === 0 ? filteredTemplates :
                activeTab === 1 ? activeTemplates : draftTemplates
              ).map((template) => (
                <Grid item xs={12} sm={6} md={4} key={template.id}>
                  <Card
                    sx={{
                      height: '100%',
                      display: 'flex',
                      flexDirection: 'column',
                      position: 'relative',
                      border: currentTemplateId === template.id ? 2 : 0,
                      borderColor: 'primary.main'
                    }}
                  >
                    <CardContent sx={{ flexGrow: 1 }}>
                      <Box display="flex" justifyContent="space-between" alignItems="flex-start">
                        <Typography variant="h6" gutterBottom>
                          {template.name}
                        </Typography>
                        <IconButton
                          size="small"
                          onClick={(e) => handleMenuOpen(e, template)}
                        >
                          <MoreIcon />
                        </IconButton>
                      </Box>

                      <Typography variant="body2" color="text.secondary" paragraph>
                        {template.description || 'No description'}
                      </Typography>

                      <Box display="flex" gap={1} flexWrap="wrap">
                        {template.isActive && (
                          <Chip
                            label="Active"
                            size="small"
                            color="success"
                            icon={<CheckIcon />}
                          />
                        )}
                        {template.isDefault && (
                          <Chip label="Default" size="small" color="primary" />
                        )}
                        <Chip label={template.platform} size="small" />
                        <Chip label={template.targetAudience} size="small" />
                        <Chip label={`v${template.version}`} size="small" variant="outlined" />
                      </Box>

                      <Box mt={2}>
                        <Typography variant="caption" color="text.secondary">
                          Created: {new Date(template.createdAt).toLocaleDateString()}
                        </Typography>
                        {template.publishedAt && (
                          <Typography variant="caption" color="text.secondary" display="block">
                            Published: {new Date(template.publishedAt).toLocaleDateString()}
                          </Typography>
                        )}
                      </Box>
                    </CardContent>

                    <CardActions>
                      <Button
                        size="small"
                        onClick={() => handleEditTemplate(template)}
                      >
                        Edit
                      </Button>
                      {!template.isActive && (
                        <Button
                          size="small"
                          color="success"
                          onClick={() => handlePublishTemplate()}
                        >
                          Publish
                        </Button>
                      )}
                    </CardActions>
                  </Card>
                </Grid>
              ))}
            </Grid>
          )}

          {filteredTemplates.length === 0 && !isLoading && (
            <Box textAlign="center" py={4}>
              <Typography variant="h6" color="text.secondary" gutterBottom>
                No templates found
              </Typography>
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={() => setShowCreateDialog(true)}
              >
                Create Your First Template
              </Button>
            </Box>
          )}
        </DialogContent>
      </Dialog>

      {/* Template Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
      >
        <MenuItem onClick={() => selectedTemplate && handleEditTemplate(selectedTemplate)}>
          <ListItemIcon>
            <EditIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>Edit</ListItemText>
        </MenuItem>
        <MenuItem onClick={handleDuplicateTemplate}>
          <ListItemIcon>
            <CopyIcon fontSize="small" />
          </ListItemIcon>
          <ListItemText>Duplicate</ListItemText>
        </MenuItem>
        {selectedTemplate && !selectedTemplate.isActive && (
          <MenuItem onClick={handlePublishTemplate}>
            <ListItemIcon>
              <PublishIcon fontSize="small" />
            </ListItemIcon>
            <ListItemText>Publish</ListItemText>
          </MenuItem>
        )}
        <Divider />
        <MenuItem onClick={handleDeleteTemplate} sx={{ color: 'error.main' }}>
          <ListItemIcon>
            <DeleteIcon fontSize="small" color="error" />
          </ListItemIcon>
          <ListItemText>Delete</ListItemText>
        </MenuItem>
      </Menu>

      {/* Create Template Dialog */}
      <CreateTemplateDialog
        open={showCreateDialog}
        onClose={() => setShowCreateDialog(false)}
        onSuccess={(template) => {
          setShowCreateDialog(false);
          handleEditTemplate(template);
        }}
      />
    </>
  );
};

export default TemplateManager;