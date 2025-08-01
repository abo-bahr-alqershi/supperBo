// Temporary placeholder for ConflictWarningModal
import React from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Typography } from '@mui/material';

interface ConflictWarningModalProps {
  open: boolean;
  conflicts: any[];
  onClose: () => void;
  onResolve: (conflict: any) => void;
  onProceedAnyway?: () => void;
}

const ConflictWarningModal: React.FC<ConflictWarningModalProps> = ({
  open,
  conflicts,
  onClose,
  onResolve,
  onProceedAnyway
}) => {
  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>تحذير: توجد تعارضات في الحجوزات</DialogTitle>
      <DialogContent>
        <Typography variant="body1" gutterBottom>
          توجد {conflicts.length} تعارضات في الحجوزات. يرجى حل هذه التعارضات قبل المتابعة.
        </Typography>
        {conflicts.map((conflict, index) => (
          <Typography key={index} variant="body2" sx={{ mb: 1 }}>
            تعارض {index + 1}: {conflict.booking_id || 'غير محدد'}
          </Typography>
        ))}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>إلغاء</Button>
        {onProceedAnyway && (
          <Button onClick={onProceedAnyway} color="warning">
            المتابعة على أي حال
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};

export default ConflictWarningModal;