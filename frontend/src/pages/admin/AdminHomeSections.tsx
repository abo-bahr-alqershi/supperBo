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
  DialogActions
} from '@mui/material';
import { ArrowUpward, ArrowDownward, Edit, Delete } from '@mui/icons-material';
import DynamicSectionForm from '../../components/admin/DynamicSectionForm';
import type { DynamicHomeSection } from '../../types/homeSections.types';
import {
  useDynamicHomeSections,
  useCreateDynamicSection,
  useUpdateDynamicSection,
  useDeleteDynamicSection,
  useReorderDynamicSections
} from '../../hooks/useDynamicSections';

const AdminHomeSections: React.FC = () => {
  // Data hooks
  const { data: sections = [], isLoading: loading, error } = useDynamicHomeSections({ includeContent: true });
  const createSection = useCreateDynamicSection();
  const updateSection = useUpdateDynamicSection();
  const deleteSection = useDeleteDynamicSection();
  const reorderSections = useReorderDynamicSections();
  // Local order state for reorder UI
  const [orderState, setOrderState] = useState<DynamicHomeSection[]>([]);

  // Sync orderState when sections update
  useEffect(() => {
    setOrderState(sections);
  }, [sections]);

  const [dialogOpen, setDialogOpen] = useState<boolean>(false);
  const [editingSection, setEditingSection] = useState<DynamicHomeSection | null>(null);
  const [formData, setFormData] = useState<any>({});
  // Handlers
  const handleOpenCreate = () => {
    setEditingSection(null);
    setFormData({});
    setDialogOpen(true);
  };

  const handleOpenEdit = (section: DynamicHomeSection) => {
    setEditingSection(section);
    setFormData({ ...section, config: section.config, content: section.content });
    setDialogOpen(true);
  };

  const handleCloseDialog = () => setDialogOpen(false);
  const handleFormChange = ({ formData }: { formData: any }) => setFormData(formData);

  const handleSave = () => {
    if (editingSection) {
      updateSection.mutate(
        { id: editingSection.id, command: formData },
        { onSuccess: () => setDialogOpen(false) }
      );
    } else {
      createSection.mutate(formData, { onSuccess: () => setDialogOpen(false) });
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Are you sure?')) return;
    deleteSection.mutate(id);
  };

  const handleMove = (index: number, direction: 'up' | 'down') => {
    const newSections = [...orderState];
    const targetIndex = direction === 'up' ? index - 1 : index + 1;
    if (targetIndex < 0 || targetIndex >= newSections.length) return;
    [newSections[index], newSections[targetIndex]] = [newSections[targetIndex], newSections[index]];
    setOrderState(newSections);
  };

  const handleSaveOrder = async () => {
    const payload = { sections: orderState.map((s, i) => ({ sectionId: s.id, newOrder: i })) };
    reorderSections.mutate(payload);
  };

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Home Sections</Typography>
        <Box>
          <Button variant="contained" color="primary" onClick={handleOpenCreate} disabled={createSection.isLoading}>Create Section</Button>
          <Button variant="outlined" color="secondary" onClick={handleSaveOrder} sx={{ ml: 1 }} disabled={reorderSections.isLoading}>Save Order</Button>
        </Box>
      </Box>
      {loading ? (
        <CircularProgress />
      ) : error ? (
        <Typography color="error">{(error as Error).message}</Typography>
      ) : (
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Order</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Title</TableCell>
              <TableCell>Active</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orderState.map((section, index) => (
              <TableRow key={section.id}>
                <TableCell>
                  {index + 1}
                  <IconButton size="small" onClick={() => handleMove(index, 'up')} disabled={index === 0}><ArrowUpward fontSize="small" /></IconButton>
                  <IconButton size="small" onClick={() => handleMove(index, 'down')} disabled={index === orderState.length - 1}><ArrowDownward fontSize="small" /></IconButton>
                </TableCell>
                <TableCell>{section.type}</TableCell>
                <TableCell>{section.title || '—'}</TableCell>
                <TableCell>{section.isActive ? 'Yes' : 'No'}</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpenEdit(section)} disabled={updateSection.isLoading}><Edit /></IconButton>
                  <IconButton onClick={() => handleDelete(section.id)} disabled={deleteSection.isLoading}><Delete /></IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="md" fullWidth>
        <DialogTitle>{editingSection ? 'Edit Section' : 'Create Section'}</DialogTitle>
        <DialogContent>
          <DynamicSectionForm formData={formData} onChange={handleFormChange} />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button variant="contained" onClick={handleSave}>Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AdminHomeSections;