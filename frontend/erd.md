erDiagram
    field_types {
        string field_type_id PK
        string name
        string display_name
        string validation_rules
        boolean is_active
        boolean is_deleted
        datetime deleted_at
    }
    property_types {
        string property_type_id PK
        string name
        string description
        string default_amenities
    }
    unit_types {
        string unit_type_id PK
        string property_type_id FK
        string name
        int max_capacity
    }
    unit_type_fields {
        string field_id PK
        string unit_type_id FK
        string field_type_id FK
        string field_name
        string display_name
        string description
        string field_options
        string validation_rules
        boolean is_required
        boolean is_searchable
        boolean is_public
        int sort_order
        string category
        boolean is_for_units
        boolean is_active
        boolean is_deleted
        datetime deleted_at
    }
    field_groups {
        string group_id PK
        string unit_type_id FK
        string group_name
        string display_name
        string description
        int sort_order
        boolean is_collapsible
        boolean is_expanded_by_default
        boolean is_deleted
        datetime deleted_at
    }
    field_group_fields {
        string field_id FK
        string group_id FK
        int sort_order
        boolean is_deleted
        datetime deleted_at
    }
    search_filters {
        string filter_id PK
        string field_id FK
        string filter_type
        string display_name
        string filter_options
        boolean is_active
        int sort_order
        boolean is_deleted
        datetime deleted_at
    }
    users {
        string user_id PK
        string name
        string email
        string password
        string phone
        string profile_image
        datetime created_at
        boolean is_active
        datetime last_login_date
        decimal total_spent
        string loyalty_tier
        boolean email_confirmed
        string email_confirmation_token
        datetime email_confirmation_token_expires
        string password_reset_token
        datetime password_reset_token_expires
        string settings_json
        string favorites_json
        datetime updated_at
        boolean is_deleted
        datetime deleted_at
    }
    roles {
        string role_id PK
        string name
    }
    user_roles {
        string user_id FK
        string role_id FK
        datetime assigned_at
    }
    admin_actions {
        string action_id PK
        string admin_id FK
        string target_id
        string target_type
        string action_type
        datetime timestamp
        string changes
    }
    notifications {
        string notification_id PK
        string type
        string title
        string message
        string title_ar
        string message_ar
        string priority
        string status
        string data
        string channels
        string sent_channels
        boolean is_read
        boolean is_dismissed
        boolean requires_action
        boolean can_dismiss
        datetime read_at
        datetime dismissed_at
        datetime scheduled_for
        datetime expires_at
        datetime delivered_at
        string group_id
        string batch_id
        string icon
        string color
        string action_url
        string action_text
        datetime created_at
    }
    amenities {
        string amenity_id PK
        string name
        string description
    }
    property_type_amenities {
        string property_type_id FK
        string amenity_id FK
        boolean is_default
    }
    property_amenities {
        string property_id FK
        string pta_id FK
        boolean is_available
        decimal extra_cost
    }
    property_policies {
        string policy_id PK
        string property_id FK
        string type
        int cancellation_window_days
        boolean require_full_payment_before_confirmation
        decimal minimum_deposit_percentage
        int min_hours_before_check_in
        string description
        string rules
    }
    properties {
        string property_id PK
        string owner_id FK
        string type_id FK
        string name
        string address
        string city
        decimal latitude
        decimal longitude
        int star_rating
        string description
        boolean is_approved
        datetime created_at
        int view_count
        int booking_count
        boolean is_deleted
        datetime deleted_at
    }
    property_services {
        string service_id PK
        string property_id FK
        string name
        decimal price
        string pricing_model
    }
    booking_services {
        string booking_id FK
        string service_id FK
        int quantity
        decimal total_price
    }
    bookings {
        string booking_id PK
        string user_id FK
        string unit_id FK
        datetime check_in
        datetime check_out
        int guests_count
        decimal total_price
        string status
        datetime booked_at
        string booking_source
        string cancellation_reason
        boolean is_walk_in
        decimal platform_commission_amount
        datetime actual_check_in_date
        datetime actual_check_out_date
        decimal final_amount
        decimal customer_rating
        string completion_notes
    }
    payments {
        string payment_id PK
        string booking_id FK
        decimal amount
        string method
        string transaction_id
        string status
        datetime payment_date
        string gateway_transaction_id
        string processed_by
    }
    unit_field_values {
        string value_id PK
        string unit_id FK
        string unit_type_field_id FK
        string field_value
        datetime created_at
        datetime updated_at
        boolean is_deleted
        datetime deleted_at
    }
    units {
        string unit_id PK
        string property_id FK
        string unit_type_id FK
        string name
        decimal base_price
        int max_capacity
        string custom_features
        boolean is_available
        int view_count
        int booking_count
        string pricing_method
    }
    reports {
        string report_id PK
        string reporter_user_id FK
        string reported_user_id
        string reported_property_id
        string reason
        string description
        datetime created_at
    }
    review_images {
        string image_id PK
        string review_id FK
        string name
        string url
        long size_bytes
        string type
        string caption
        string alt_text
        string tags
        boolean is_main
        int display_order
        datetime uploaded_at
        string status
    }
    reviews {
        string review_id PK
        string booking_id FK
        int cleanliness
        int service
        int location
        int value
        string comment
        datetime created_at
        string response_text
        datetime response_date
        boolean is_pending_approval
        string property_id FK
    }
    audit_logs {
        string log_id PK
        string entity_type
        string entity_id
        string action
        string old_values
        string new_values
        string performed_by
        string username
        string ip_address
        string user_agent
        string notes
        string metadata
        boolean is_successful
        string error_message
        long duration_ms
        string session_id
        string request_id
        string source
        datetime created_at
    }

    field_types ||--o{ unit_type_fields : defines_type
    unit_types ||--o{ unit_type_fields : has_fields
    unit_types ||--o{ field_groups : has_groups
    field_groups ||--o{ field_group_fields : groups_fields
    unit_type_fields ||--o{ field_group_fields : field_in_group
    unit_type_fields ||--o{ search_filters : used_in_filters
    unit_type_fields ||--o{ unit_field_values : stores_values
    units ||--o{ unit_field_values : has_values
    properties ||--o{ units : has_units
    property_types ||--o{ property_type_amenities : has_amenities
    property_type_amenities ||--o{ property_amenities : has_amenities
    properties ||--o{ property_services : has_services
    property_services ||--o{ booking_services : booked_services
    users ||--o{ bookings : makes_bookings
    users ||--o{ user_roles : has_roles
    roles ||--o{ user_roles : assigned_users
    users ||--o{ admin_actions : performed_actions
    bookings ||--o{ payments : has_payments
    bookings ||--o{ booking_services : includes_services
    properties ||--o{ property_policies : has_policies
    users ||--o{ reports : filed_reports
    properties ||--o{ reports : received_reports
    bookings ||--o{ reviews : has_reviews
    reviews ||--o{ review_images : has_images
    users ||--o{ notifications : received_notifications
    users ||--o{ notifications : sent_notifications
    users ||--o{ payments : processed_payments
    users ||--o{ properties : owns_properties
    property_types ||--o{ unit_types : has_unit_types
    property_types ||--o{ properties : has_properties
