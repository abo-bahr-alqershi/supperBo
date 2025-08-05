import React, { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  IconButton,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField
} from '@mui/material';
import { Edit } from '@mui/icons-material';
import {
  useHomeConfig,
  useCreateHomeConfig,
  useUpdateHomeConfig,
  usePublishHomeConfig
} from '../../hooks/useHomeConfig';
import type { DynamicHomeConfig } from '../../types/homeSections.types';

const AdminHomeConfig: React.FC = () => {
  const { data: config, isLoading, error, refetch } = useHomeConfig();
  const createConfig = useCreateHomeConfig();
  const updateConfig = useUpdateHomeConfig();
  const publishConfig = usePublishHomeConfig();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [formData, setFormData] = useState<DynamicHomeConfig | null>(null);

  const handleOpen = () => {
    setFormData(config || null);
    setDialogOpen(true);
  };

  const handleClose = () => setDialogOpen(false);

  const handleSave = () => {
    if (!formData) return;
    if (config) {
      updateConfig.mutate({ id: config.id, command: formData });
    } else {
      createConfig.mutate(formData);
    }
    setDialogOpen(false);
    refetch();
  };

  const handlePublish = () => {
    if (!config) return;
    publishConfig.mutate({ id: config.id });
    refetch();
  };

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h6">Home Config</Typography>
        <Box>
          <Button variant="contained" color="primary" onClick={handleOpen} disabled={isLoading}>Edit Config</Button>
          <Button variant="outlined" color="secondary" onClick={handlePublish} sx={{ ml: 1 }} disabled={!config}>Publish</Button>
        </Box>
      </Box>
      {isLoading ? (
        <CircularProgress />
      ) : error ? (
        <Typography color="error">{(error as Error).message}</Typography>
      ) : config ? (
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Version</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Published At</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            <TableRow>
              <TableCell>{config.version}</TableCell>
              <TableCell>{config.isActive ? 'Yes' : 'No'}</TableCell>
              <TableCell>{config.publishedAt || '—'}</TableCell>
            </TableRow>
          </TableBody>
        </Table>
      ) : (
        <Typography>No configuration found.</Typography>
      )}
      <Dialog open={dialogOpen} onClose={handleClose} maxWidth="md" fullWidth>
        <DialogTitle>Edit Home Config</DialogTitle>
        <DialogContent>
          <TextField
            label="Global Settings (JSON)"
            multiline
            fullWidth
            minRows={6}
            value={JSON.stringify(formData?.globalSettings, null, 2)}
            onChange={e => setFormData(prev => prev && { ...prev, globalSettings: JSON.parse(e.target.value) })}
            margin="normal"
          />
          <TextField
            label="Theme Settings (JSON)"
            multiline
            fullWidth
            minRows={6}
            value={JSON.stringify(formData?.themeSettings, null, 2)}
            onChange={e => setFormData(prev => prev && { ...prev, themeSettings: JSON.parse(e.target.value) })}
            margin="normal"
          />
          {/* Add other setting fields similarly */}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AdminHomeConfig;