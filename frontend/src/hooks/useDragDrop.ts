import { useState, useCallback, useRef } from 'react';
import { useDrag, useDrop, DragSourceMonitor, DropTargetMonitor } from 'react-dnd';
import type { DragItem, DragState, Position } from '../types/dragDrop.types';

export const useDragDrop = () => {
  const [dragState, setDragState] = useState<DragState>({
    isDragging: false,
    draggedItem: null,
    draggedOverTarget: null,
    draggedOverIndex: null
  });

  const dragCounter = useRef(0);

  const handleDragStart = useCallback((item: DragItem) => {
    setDragState({
      isDragging: true,
      draggedItem: item,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
  }, []);

  const handleDragEnd = useCallback(() => {
    setDragState({
      isDragging: false,
      draggedItem: null,
      draggedOverTarget: null,
      draggedOverIndex: null
    });
    dragCounter.current = 0;
  }, []);

  const handleDragEnter = useCallback((targetId: string, index?: number) => {
    dragCounter.current++;
    setDragState(prev => ({
      ...prev,
      draggedOverTarget: targetId,
      draggedOverIndex: index ?? null
    }));
  }, []);

  const handleDragLeave = useCallback(() => {
    dragCounter.current--;
    if (dragCounter.current === 0) {
      setDragState(prev => ({
        ...prev,
        draggedOverTarget: null,
        draggedOverIndex: null
      }));
    }
  }, []);

  const useDraggable = useCallback((
    item: DragItem,
    options?: {
      canDrag?: () => boolean;
      preview?: React.ReactNode;
    }
  ) => {
    const [{ isDragging }, drag, preview] = useDrag({
      type: item.type,
      item: () => {
        handleDragStart(item);
        return item;
      },
      end: handleDragEnd,
      canDrag: options?.canDrag,
      collect: (monitor: DragSourceMonitor) => ({
        isDragging: monitor.isDragging()
      })
    });

    return { drag, preview, isDragging };
  }, [handleDragStart, handleDragEnd]);

  const useDroppable = useCallback((
    targetId: string,
    acceptTypes: string[],
    onDrop: (item: DragItem, index?: number) => void,
    options?: {
      canDrop?: (item: DragItem) => boolean;
      onHover?: (item: DragItem, index?: number) => void;
    }
  ) => {
    const [{ isOver, canDrop }, drop] = useDrop({
      accept: acceptTypes,
      drop: (item: DragItem, monitor: DropTargetMonitor) => {
        if (monitor.isOver({ shallow: true })) {
          onDrop(item, dragState.draggedOverIndex ?? undefined);
          handleDragEnd();
        }
      },
      canDrop: (item: DragItem) => options?.canDrop?.(item) ?? true,
      hover: (item: DragItem, monitor: DropTargetMonitor) => {
        if (monitor.isOver({ shallow: true })) {
          handleDragEnter(targetId);
          options?.onHover?.(item);
        }
      },
      collect: (monitor: DropTargetMonitor) => ({
        isOver: monitor.isOver({ shallow: true }),
        canDrop: monitor.canDrop()
      })
    });

    return { drop, isOver, canDrop };
  }, [dragState.draggedOverIndex, handleDragEnd, handleDragEnter]);

  const getDropIndex = useCallback((
    e: React.DragEvent,
    orientation: 'horizontal' | 'vertical',
    itemCount: number
  ): number => {
    const rect = e.currentTarget.getBoundingClientRect();
    const pos = orientation === 'horizontal' 
      ? (e.clientX - rect.left) / rect.width
      : (e.clientY - rect.top) / rect.height;
    
    return Math.min(Math.floor(pos * itemCount), itemCount - 1);
  }, []);

  return {
    dragState,
    useDraggable,
    useDroppable,
    getDropIndex,
    handleDragEnd
  };
};
