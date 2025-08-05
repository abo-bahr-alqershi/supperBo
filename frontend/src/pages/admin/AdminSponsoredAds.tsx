import React, { useState, useEffect } from 'react';
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
import { Edit, Delete } from '@mui/icons-material';
import {
  useSponsoredAds,
  useCreateSponsoredAd,
  useUpdateSponsoredAd,
  useRecordAdInteraction
} from '../../hooks/useSponsoredAds';
import type { SponsoredAd } from '../../types/homeSections.types';

const AdminSponsoredAds: React.FC = () => {
  const { data = [], isLoading, error, refetch } = useSponsoredAds({});
  const createAd = useCreateSponsoredAd();
  const updateAd = useUpdateSponsoredAd();
  const recordInteraction = useRecordAdInteraction();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<SponsoredAd | null>(null);
  const [formData, setFormData] = useState<Partial<SponsoredAd>>({});

  useEffect(() => {
    if (editing) setFormData(editing);
  }, [editing]);

  const handleOpenCreate = () => {
    setEditing(null);
    setFormData({});
    setDialogOpen(true);
  };

  const handleOpenEdit = (ad: SponsoredAd) => {
    setEditing(ad);
    setFormData(ad);
    setDialogOpen(true);
  };

  const handleClose = () => setDialogOpen(false);

  const handleSave = () => {
    if (editing) {
      updateAd.mutate({ id: editing.id, command: formData as any }, { onSuccess: () => { setDialogOpen(false); refetch(); } });
    } else {
      createAd.mutate(formData as any, { onSuccess: () => { setDialogOpen(false); refetch(); } });
    }
  };

  const handleDelete = (id: string) => {
    if (!window.confirm('Delete ad?')) return;
    // Could call delete hook if implemented
  };

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h6">Sponsored Ads</Typography>
        <Button variant="contained" color="primary" onClick={handleOpenCreate} disabled={isLoading}>Add Ad</Button>
      </Box>
      {isLoading ? (
        <CircularProgress />
      ) : error ? (
        <Typography color="error">{(error as Error).message}</Typography>
      ) : (
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Title</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Impressions</TableCell>
              <TableCell>Clicks</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data.map(ad => (
              <TableRow key={ad.id}>
                <TableCell>{ad.title}</TableCell>
                <TableCell>{ad.isActive ? 'Yes' : 'No'}</TableCell>
                <TableCell>{ad.impressionCount}</TableCell>
                <TableCell>{ad.clickCount}</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpenEdit(ad)} disabled={updateAd.status === 'pending'}><Edit /></IconButton>
                  <IconButton onClick={() => handleDelete(ad.id)}><Delete /></IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
      <Dialog open={dialogOpen} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>{editing ? 'Edit Ad' : 'Add Ad'}</DialogTitle>
        <DialogContent>
          <TextField label="Title" fullWidth value={formData.title || ''} onChange={e => setFormData(prev => ({ ...prev, title: e.target.value }))} margin="normal" />
          <TextField label="Subtitle" fullWidth value={formData.subtitle || ''} onChange={e => setFormData(prev => ({ ...prev, subtitle: e.target.value }))} margin="normal" />
          {/* Add other fields like description, imageUrl, ctaText, etc. */}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AdminSponsoredAds;