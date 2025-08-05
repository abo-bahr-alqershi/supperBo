
# AI Agent Specification: Dynamic Home Screen Section Management

## 1. Overview

This document outlines the requirements for an AI agent to build a new, powerful, and compatible management system for the dynamic home screen sections. The system should be fully integrated with the existing backend, frontend, and mobile application.

## 2. Core Concepts

The home screen is composed of dynamic "sections". Each section has a specific type, content, and configuration. The goal is to provide a flexible system for administrators to create, manage, and reorder these sections, with changes reflected in both the web and mobile applications.

### 2.1. Key Entities

- **`DynamicHomeSection`**: The main entity representing a section on the home screen.
- **`DynamicSectionContent`**: The content within a section (e.g., a specific property, an offer, a destination).
- **`DynamicHomeConfig`**: Global configuration for the home screen, including feature flags and theme settings.
- **`SectionType`**: An enum that defines the type of a section, which determines its layout and behavior.

## 3. Backend Analysis (`.cs` files)

The backend is built with ASP.NET Core and uses MediatR for handling commands and queries.

### 3.1. Endpoints

The primary controller is `HomeSectionsController.cs`. It exposes the following endpoints under the `/api/admin/home-sections/` route:

- **Dynamic Sections:**
  - `GET /dynamic-sections`: Fetches all dynamic sections.
  - `POST /dynamic-sections`: Creates a new dynamic section.
  - `PUT /dynamic-sections/{id}`: Updates an existing section.
  - `POST /dynamic-sections/{id}/toggle`: Toggles the active status of a section.
  - `DELETE /dynamic-sections/{id}`: Deletes a section.
  - `POST /dynamic-sections/reorder`: Reorders the sections.

### 3.2. Data Transfer Objects (DTOs)

- **`DynamicHomeSectionDto.cs`**: Defines the data structure for a section when it's exposed through the API.
- **`CreateDynamicHomeSectionCommand.cs`**: The command used to create a new section.

### 3.3. Core Entities

- **`DynamicHomeSection.cs`**: The core database entity for a section. It includes properties like `SectionType`, `Order`, `Title`, `Subtitle`, `SectionConfig` (JSON), `Metadata` (JSON), `ScheduledAt`, `ExpiresAt`, and `TargetAudience`.

**Recommendation for AI Agent:** The agent should be able to interact with all these endpoints and understand the structure of the DTOs and entities. It should be able to generate valid JSON for `SectionConfig` and `Metadata` based on the `SectionType`.

## 4. Frontend Analysis (`.tsx`, `.ts` files)

The frontend is a React application built with Vite, using Material-UI for components and React Query for data fetching.

### 4.1. Key Components

- **`AdminHomeSectionsContent.tsx`**: The main component for managing the home sections. It displays a table of sections and provides buttons for creating, editing, deleting, and reordering them.
- **`DynamicSectionForm.tsx`**: A form for creating and editing sections. It uses `react-jsonschema-form` to dynamically generate the form based on a JSON schema.
- **`DynamicSection.tsx`**: A component that renders a live preview of a section.

### 4.2. Hooks and Services

- **`useDynamicSections.ts`**: A custom hook that provides functions for interacting with the backend API (fetching, creating, updating, deleting, reordering).
- **`homeSectionsService.ts`**: A service that makes the actual API calls to the backend.

### 4.3. Types

- **`homeSections.types.ts`**: Contains all the TypeScript types for the home screen sections, including `DynamicHomeSection`, `DynamicContent`, and the various command types.

**Recommendation for AI Agent:** The agent should be able to understand the existing frontend components and how they interact with the backend. It should be able to generate new components and extend the existing ones. The agent should also be able to work with the existing hooks and services.

## 5. Mobile App Analysis (`.dart` files)

The mobile app is a Flutter application.

### 5.1. Key Components

- **`dynamic_section_widget.dart`**: A widget that renders a dynamic section. It's a placeholder and needs to be implemented to render different widgets based on the `section.type`.

### 5.2. Enums and Models

- **`section_type_enum.dart`**: Defines the `SectionType` enum, which is consistent with the backend and frontend.
- **`home_section_model.dart`**: The data model for a home section, including JSON serialization/deserialization logic.
- **`home_section.dart`**: The domain entity for a home section.

### 5.3. Use Cases

- **`get_home_sections_usecase.dart`**: The use case for fetching the home sections from the repository.

**Recommendation for AI Agent:** The agent needs to be able to implement the `DynamicSectionWidget` to render the different section types. It should also be able to create new widgets for each section type.

## 6. AI Agent Instructions

The new AI agent should be able to perform the following tasks:

1.  **Understand the existing codebase:** The agent must be able to read and analyze the provided files to understand the current implementation.
2.  **Build a new management system:** The agent should build a new, powerful, and compatible management system for the dynamic home screen sections.
3.  **Full-stack compatibility:** The new system must be fully compatible with the existing backend, frontend, and mobile application.
4.  **Utilize existing components:** The agent should use the existing frontend components (like tables, forms, etc.) to improve the workflow.
5.  **Request information:** The agent should be able to request any additional information it needs by asking to read specific files.

## 7. Frontend Component Tree

The following is a tree of the frontend components that the AI agent can use to improve the workflow:

```
frontend/src/components
├── admin
│   ├── DynamicSectionForm.tsx
│   └── ... (other admin components)
├── home
│   ├── Common
│   │   └── SectionHeader.tsx
│   ├── DynamicSection.tsx
│   └── HomeScreenLayout.tsx
└── ... (other shared components)
```

The agent should be instructed to use these components as a starting point and to create new components as needed.
