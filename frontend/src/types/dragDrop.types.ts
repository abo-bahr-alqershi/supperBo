export interface DragItem {
  id: string;
  type: DragItemType;
  componentType?: string;
  sourceIndex: number;
  sourceSectionId?: string;
  data: any;
}

export interface DropTarget {
  id: string;
  type: DropTargetType;
  accept: DragItemType[];
  canDrop: (item: DragItem) => boolean;
  onDrop: (item: DragItem, targetIndex?: number) => void;
}

export interface DragPreview {
  width: number;
  height: number;
  content: React.ReactNode;
}

export interface DragState {
  isDragging: boolean;
  draggedItem: DragItem | null;
  draggedOverTarget: string | null;
  draggedOverIndex: number | null;
}

export type DragItemType = 'new-component' | 'existing-component' | 'section';
export type DropTargetType = 'section' | 'canvas' | 'component-list';

export interface Position {
  x: number;
  y: number;
}

export interface Size {
  width: number;
  height: number;
}

export interface GridPosition {
  row: number;
  col: number;
  rowSpan: number;
  colSpan: number;
}