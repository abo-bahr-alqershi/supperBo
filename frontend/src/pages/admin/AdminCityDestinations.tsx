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
  useCityDestinations,
  useCreateCityDestination,
  useUpdateCityDestination,
  useUpdateCityDestinationStats
} from '../../hooks/useCityDestinations';
import type { CityDestination } from '../../types/homeSections.types';

const AdminCityDestinations: React.FC = () => {
  const { data = [], isLoading, error, refetch } = useCityDestinations({});
  const createDest = useCreateCityDestination();
  const updateDest = useUpdateCityDestination();
  const updateStats = useUpdateCityDestinationStats();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<CityDestination | null>(null);
  const [formData, setFormData] = useState<Partial<CityDestination>>({});

  useEffect(() => {
    if (editing) setFormData(editing);
  }, [editing]);

  const handleOpenCreate = () => {
    setEditing(null);
    setFormData({});
    setDialogOpen(true);
  };

  const handleOpenEdit = (dest: CityDestination) => {
    setEditing(dest);
    setFormData(dest);
    setDialogOpen(true);
  };

  const handleClose = () => setDialogOpen(false);

  const handleSave = () => {
    if (editing) {
      updateDest.mutate({ id: editing.id, command: formData as any }, { onSuccess: () => { setDialogOpen(false); refetch(); } });
    } else {
      createDest.mutate(formData as any, { onSuccess: () => { setDialogOpen(false); refetch(); } });
    }
  };

  const handleDelete = (id: string) => {
    if (!window.confirm('Delete destination?')) return;
    // Could call a delete hook if exists
  };

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h6">City Destinations</Typography>
        <Button variant="contained" color="primary" onClick={handleOpenCreate} disabled={isLoading}>Add Destination</Button>
      </Box>
      {isLoading ? (
        <CircularProgress />
      ) : error ? (
        <Typography color="error">{(error as Error).message}</Typography>
      ) : (
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Country</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Stats</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data.map(dest => (
              <TableRow key={dest.id}>
                <TableCell>{dest.name}</TableCell>
                <TableCell>{dest.country}</TableCell>
                <TableCell>{dest.isActive ? 'Yes' : 'No'}</TableCell>
                <TableCell>{dest.propertyCount} properties</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpenEdit(dest)} disabled={updateDest.status === 'pending'}><Edit /></IconButton>
                  <IconButton onClick={() => handleDelete(dest.id)}><Delete /></IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
      <Dialog open={dialogOpen} onClose={handleClose} maxWidth="sm" fullWidth>
        <DialogTitle>{editing ? 'Edit Destination' : 'Add Destination'}</DialogTitle>
        <DialogContent>
          <TextField label="Name" fullWidth value={formData.name || ''} onChange={e => setFormData(prev => ({ ...prev, name: e.target.value }))} margin="normal" />
          <TextField label="Country" fullWidth value={formData.country || ''} onChange={e => setFormData(prev => ({ ...prev, country: e.target.value }))} margin="normal" />
          {/* Add other fields like description, images, stats updating form */}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AdminCityDestinations;