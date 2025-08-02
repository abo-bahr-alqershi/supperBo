import { useState, useCallback, useRef, useEffect } from 'react';
import type { DragItem, DragState, Position } from '../types/dragDrop.types';
import { createDragPreview, updateDragPreview, removeDragPreview } from '../utils/dragDropUtils';

interface UseDragDropOptions {
  onDragStart?: (item: DragItem) => void;
  onDragEnd?: (item: DragItem | null) => void;
  onDrop?: (item: DragItem, targetId: string, position?: Position) => void;
}

export const useDragDrop = (options: UseDragDropOptions = {}) => {
  const [dragState, setDragState] = useState<DragState>({
    isDragging: false,
    draggedItem: null,
    draggedOverTarget: null,
    draggedOverIndex: null
  });
  
  const dragPreviewRef = useRef<HTMLDivElement | null>(null);
  const dragStartPositionRef = useRef<Position>({ x: 0, y: 0 });
  
  const handleDragStart = useCallback((
    e: React.DragEvent | MouseEvent,
    item: DragItem
  ) => {
    if ('dataTransfer' in e) {
      e.dataTransfer.effectAllowed = 'move';
      e.dataTransfer.setData('application/json', JSON.stringify(item));
      
      // Create custom drag image
      const target = e.target as HTMLElement;
      const preview = createDragPreview(target);
      dragPreviewRef.current = preview;
      
      // Hide default drag image
      const emptyImg = new Image();
      e.dataTransfer.setDragImage(emptyImg, 0, 0);
    }
    
    dragStartPositionRef.current = {
      x: 'clientX' in e ? e.clientX : 0,
      y: 'clientY' in e ? e.clientY : 0
    };
    
    setDragState({
      isDragging: true,
      draggedItem: item,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
    
    options.onDragStart?.(item);
  }, [options]);
  
  const handleDragEnd = useCallback((e: React.DragEvent | MouseEvent) => {
    if (dragPreviewRef.current) {
      removeDragPreview(dragPreviewRef.current);
      dragPreviewRef.current = null;
    }
    
    setDragState({
      isDragging: false,
      draggedItem: null,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
    
    options.onDragEnd?.(dragState.draggedItem);
  }, [dragState.draggedItem, options]);
  
  const handleDragOver = useCallback((
    e: React.DragEvent,
    targetId: string,
    canAccept: boolean = true
  ) => {
    e.preventDefault();
    
    if (!canAccept) {
      e.dataTransfer.dropEffect = 'none';
      return;
    }
    
    e.dataTransfer.dropEffect = 'move';
    
    setDragState(prev => ({
      ...prev,
      draggedOverTarget: targetId
    }));
    
    // Update preview position
    if (dragPreviewRef.current) {
      updateDragPreview(dragPreviewRef.current, e.clientX, e.clientY);
    }
  }, []);
  
  const handleDragLeave = useCallback((e: React.DragEvent, targetId: string) => {
    // Check if we're leaving the target completely
    const relatedTarget = e.relatedTarget as HTMLElement;
    const currentTarget = e.currentTarget as HTMLElement;
    
    if (!currentTarget.contains(relatedTarget)) {
      setDragState(prev => ({
        ...prev,
        draggedOverTarget: prev.draggedOverTarget === targetId ? null : prev.draggedOverTarget
      }));
    }
  }, []);
  
  const handleDrop = useCallback((
    e: React.DragEvent,
    targetId: string
  ) => {
    e.preventDefault();
    e.stopPropagation();
    
    const dataString = e.dataTransfer.getData('application/json');
    if (!dataString) return;
    
    try {
      const item = JSON.parse(dataString) as DragItem;
      const position: Position = {
        x: e.clientX,
        y: e.clientY
      };
      
      options.onDrop?.(item, targetId, position);
    } catch (error) {
      console.error('Failed to parse drag data:', error);
    }
    
    handleDragEnd(e);
  }, [options, handleDragEnd]);
  
  // Mouse-based drag for better preview control
  const handleMouseDown = useCallback((
    e: React.MouseEvent,
    item: DragItem
  ) => {
    e.preventDefault();
    
    const startX = e.clientX;
    const startY = e.clientY;
    let isDragging = false;
    
    const handleMouseMove = (moveEvent: MouseEvent) => {
      const distance = Math.sqrt(
        Math.pow(moveEvent.clientX - startX, 2) +
        Math.pow(moveEvent.clientY - startY, 2)
      );
      
      if (!isDragging && distance > 5) {
        isDragging = true;
        handleDragStart(moveEvent, item);
      }
      
      if (isDragging && dragPreviewRef.current) {
        updateDragPreview(dragPreviewRef.current, moveEvent.clientX, moveEvent.clientY);
      }
    };
    
    const handleMouseUp = (upEvent: MouseEvent) => {
      if (isDragging) {
        handleDragEnd(upEvent);
      }
      
      document.removeEventListener('mousemove', handleMouseMove);
      document.removeEventListener('mouseup', handleMouseUp);
    };
    
    document.addEventListener('mousemove', handleMouseMove);
    document.addEventListener('mouseup', handleMouseUp);
  }, [handleDragStart, handleDragEnd]);
  
  // Touch support
  const handleTouchStart = useCallback((
    e: React.TouchEvent,
    item: DragItem
  ) => {
    const touch = e.touches[0];
    const startX = touch.clientX;
    const startY = touch.clientY;
    let isDragging = false;
    
    const handleTouchMove = (moveEvent: TouchEvent) => {
      const touch = moveEvent.touches[0];
      const distance = Math.sqrt(
        Math.pow(touch.clientX - startX, 2) +
        Math.pow(touch.clientY - startY, 2)
      );
      
      if (!isDragging && distance > 5) {
        isDragging = true;
        handleDragStart(moveEvent as any, item);
      }
      
      if (isDragging && dragPreviewRef.current) {
        updateDragPreview(dragPreviewRef.current, touch.clientX, touch.clientY);
      }
    };
    
    const handleTouchEnd = (endEvent: TouchEvent) => {
      if (isDragging) {
        handleDragEnd(endEvent as any);
      }
      
      document.removeEventListener('touchmove', handleTouchMove);
      document.removeEventListener('touchend', handleTouchEnd);
    };
    
    document.addEventListener('touchmove', handleTouchMove);
    document.addEventListener('touchend', handleTouchEnd);
  }, [handleDragStart, handleDragEnd]);
  
  // Global mouse move handler for drag preview
  useEffect(() => {
    if (!dragState.isDragging || !dragPreviewRef.current) return;
    
    const handleGlobalMouseMove = (e: MouseEvent) => {
      if (dragPreviewRef.current) {
        updateDragPreview(dragPreviewRef.current, e.clientX, e.clientY);
      }
    };
    
    document.addEventListener('mousemove', handleGlobalMouseMove);
    return () => document.removeEventListener('mousemove', handleGlobalMouseMove);
  }, [dragState.isDragging]);
  
  return {
    dragState,
    
    // HTML5 Drag API handlers
    handleDragStart,
    handleDragEnd,
    handleDragOver,
    handleDragLeave,
    handleDrop,
    
    // Mouse/Touch handlers for better control
    handleMouseDown,
    handleTouchStart,
    
    // Utilities
    isDragging: dragState.isDragging,
    draggedItem: dragState.draggedItem,
    isDraggedOver: (targetId: string) => dragState.draggedOverTarget === targetId,
    
    // Helper functions
    setDraggedOverIndex: (index: number | null) => 
      setDragState(prev => ({ ...prev, draggedOverIndex: index }))
  };
};