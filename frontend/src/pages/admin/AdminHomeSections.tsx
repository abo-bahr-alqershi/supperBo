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
import homeSectionsService from '../../services/homeSectionsService';
import DynamicSectionForm from '../../components/admin/DynamicSectionForm';
import type { DynamicHomeSection } from '../../types/homeSections.types';

const AdminHomeSections: React.FC = () => {
  const [sections, setSections] = useState<DynamicHomeSection[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string>('');
  const [dialogOpen, setDialogOpen] = useState<boolean>(false);
  const [editingSection, setEditingSection] = useState<DynamicHomeSection | null>(null);
  const [formData, setFormData] = useState<any>({});

  const fetchSections = async () => {
    setLoading(true);
    try {
      const data = await homeSectionsService.getDynamicSections({ includeContent: true });
      setSections(data.sort((a, b) => a.order - b.order));
    } catch (err: any) {
      setError(err.message || 'Failed to load sections');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchSections(); }, []);

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

  const handleSave = async () => {
    try {
      if (editingSection) {
        await homeSectionsService.updateDynamicSection(editingSection.id, formData);
      } else {
        await homeSectionsService.createDynamicSection(formData);
      }
      await fetchSections();
      setDialogOpen(false);
    } catch (err: any) {
      console.error(err);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm('Are you sure?')) return;
    try {
      await homeSectionsService.deleteDynamicSection(id);
      await fetchSections();
    } catch (err: any) {
      console.error(err);
    }
  };

  const handleMove = (index: number, direction: 'up' | 'down') => {
    const newSections = [...sections];
    const targetIndex = direction === 'up' ? index - 1 : index + 1;
    if (targetIndex < 0 || targetIndex >= newSections.length) return;
    [newSections[index], newSections[targetIndex]] = [newSections[targetIndex], newSections[index]];
    setSections(newSections);
  };

  const handleSaveOrder = async () => {
    try {
      const payload = { sections: sections.map((s, i) => ({ sectionId: s.id, newOrder: i })) };
      await homeSectionsService.reorderDynamicSections(payload);
      await fetchSections();
    } catch (err: any) {
      console.error(err);
    }
  };

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h5">Home Sections</Typography>
        <Box>
          <Button variant="contained" color="primary" onClick={handleOpenCreate}>Create Section</Button>
          <Button variant="outlined" color="secondary" onClick={handleSaveOrder} sx={{ ml: 1 }}>Save Order</Button>
        </Box>
      </Box>
      {loading ? (
        <CircularProgress />
      ) : error ? (
        <Typography color="error">{error}</Typography>
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
            {sections.map((section, index) => (
              <TableRow key={section.id}>
                <TableCell>
                  {section.order}
                  <IconButton size="small" onClick={() => handleMove(index, 'up')} disabled={index === 0}><ArrowUpward fontSize="small" /></IconButton>
                  <IconButton size="small" onClick={() => handleMove(index, 'down')} disabled={index === sections.length - 1}><ArrowDownward fontSize="small" /></IconButton>
                </TableCell>
                <TableCell>{section.type}</TableCell>
                <TableCell>{section.title || '—'}</TableCell>
                <TableCell>{section.isActive ? 'Yes' : 'No'}</TableCell>
                <TableCell>
                  <IconButton onClick={() => handleOpenEdit(section)}><Edit /></IconButton>
                  <IconButton onClick={() => handleDelete(section.id)}><Delete /></IconButton>
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