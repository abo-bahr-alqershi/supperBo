# معمارية تطبيق Flutter - Yemen Booking

lib/
├── main.dart
├── app.dart
├── injection_container.dart  # Dependency Injection
│
├── core/
│   ├── constants/
│   │   ├── api_constants.dart
│   │   ├── app_constants.dart
│   │   ├── storage_constants.dart
│   │   ├── route_constants.dart
│   │   └── component_types.dart  # ✨ جديد - أنواع المكونات
│   │
│   ├── theme/
│   │   ├── app_theme.dart
│   │   ├── app_colors.dart
│   │   ├── app_text_styles.dart
│   │   └── app_dimensions.dart
│   │
│   ├── localization/
│   │   ├── app_localizations.dart
│   │   ├── l10n/
│   │   │   ├── app_ar.arb
│   │   │   └── app_en.arb
│   │   └── locale_manager.dart
│   │
│   ├── network/
│   │   ├── api_client.dart
│   │   ├── api_interceptors.dart
│   │   ├── api_exceptions.dart
│   │   └── network_info.dart
│   │
│   ├── error/
│   │   ├── failures.dart
│   │   ├── exceptions.dart
│   │   └── error_handler.dart
│   │
│   ├── utils/
│   │   ├── validators.dart
│   │   ├── formatters.dart
│   │   ├── date_utils.dart
│   │   ├── price_calculator.dart
│   │   ├── location_utils.dart
│   │   ├── style_generator.dart  # ✨ جديد
│   │   ├── component_builder.dart  # ✨ جديد
│   │   └── dynamic_data_parser.dart  # ✨ جديد
│   │   
│   ├── models/
│   │   ├── paginated_result.dart
│   │   ├── result_dto.dart
│   │   └── base_component_data.dart  # ✨ جديد
│   │
│   └── widgets/
│       ├── app_bar_widget.dart
│       ├── loading_widget.dart
│       ├── error_widget.dart
│       ├── empty_widget.dart
│       ├── cached_image_widget.dart
│       ├── rating_widget.dart
│       ├── price_widget.dart
│       └── dynamic/  # ✨ جديد - مجلد للمكونات الديناميكية
│           ├── base_dynamic_widget.dart
│           └── dynamic_widget_factory.dart
│
├── features/
├── features/
│   ├── dynamic_home/  # ✨ جديد - ميزة الشاشة الرئيسية الديناميكية
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── home_screen_local_datasource.dart
│   │   │   │   └── home_screen_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── home_screen_template_model.dart
│   │   │   │   ├── home_screen_section_model.dart
│   │   │   │   ├── home_screen_component_model.dart
│   │   │   │   ├── component_property_model.dart
│   │   │   │   ├── component_style_model.dart
│   │   │   │   ├── component_action_model.dart
│   │   │   │   └── component_data_source_model.dart
│   │   │   └── repositories/
│   │   │       └── dynamic_home_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── home_screen_template.dart
│   │   │   │   ├── home_screen_section.dart
│   │   │   │   ├── home_screen_component.dart
│   │   │   │   ├── component_property.dart
│   │   │   │   ├── component_style.dart
│   │   │   │   ├── component_action.dart
│   │   │   │   └── component_data_source.dart
│   │   │   ├── repositories/
│   │   │   │   └── dynamic_home_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_active_home_screen_usecase.dart
│   │   │       ├── get_component_data_usecase.dart
│   │   │       ├── refresh_home_screen_usecase.dart
│   │   │       └── track_component_interaction_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── dynamic_home_bloc.dart
│   │       │   ├── dynamic_home_event.dart
│   │       │   └── dynamic_home_state.dart
│   │       ├── pages/
│   │       │   └── dynamic_home_page.dart
│   │       ├── widgets/
│   │       │   ├── dynamic_section_widget.dart
│   │       │   ├── dynamic_component_wrapper.dart
│   │       │   └── components/  # ✨ المكونات الديناميكية
│   │       │       ├── banner_component.dart
│   │       │       ├── carousel_component.dart
│   │       │       ├── category_grid_component.dart
│   │       │       ├── property_list_component.dart
│   │       │       ├── search_bar_component.dart
│   │       │       ├── offer_card_component.dart
│   │       │       ├── text_block_component.dart
│   │       │       ├── image_gallery_component.dart
│   │       │       ├── map_view_component.dart
│   │       │       └── filter_bar_component.dart
│   │       └── handlers/
│   │           ├── action_handler.dart
│   │           └── navigation_handler.dart
│   │
│   ├── auth/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── auth_local_datasource.dart
│   │   │   │   └── auth_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── user_model.dart
│   │   │   │   └── auth_response_model.dart
│   │   │   └── repositories/
│   │   │       └── auth_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── user.dart
│   │   │   │   └── auth_response.dart
│   │   │   ├── repositories/
│   │   │   │   └── auth_repository.dart
│   │   │   └── usecases/
│   │   │       ├── login_usecase.dart
│   │   │       ├── register_usecase.dart
│   │   │       ├── logout_usecase.dart
│   │   │       └── reset_password_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── auth_bloc.dart
│   │       │   ├── auth_event.dart
│   │       │   └── auth_state.dart
│   │       ├── pages/
│   │       │   ├── login_page.dart
│   │       │   ├── register_page.dart
│   │       │   ├── forgot_password_page.dart
│   │       │   └── profile_page.dart
│   │       └── widgets/
│   │           ├── login_form.dart
│   │           ├── register_form.dart
│   │           └── social_login_buttons.dart
│   │
│   ├── home/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── home_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_model.dart
│   │   │   │   └── featured_property_model.dart
│   │   │   └── repositories/
│   │   │       └── home_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── featured_property.dart
│   │   │   ├── repositories/
│   │   │   │   └── home_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_featured_properties_usecase.dart
│   │   │       ├── get_nearby_properties_usecase.dart
│   │   │       └── get_popular_destinations_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── home_bloc.dart
│   │       │   ├── home_event.dart
│   │       │   └── home_state.dart
│   │       ├── pages/
│   │       │   └── home_page.dart
│   │       └── widgets/
│   │           ├── search_bar_widget.dart
│   │           ├── featured_properties_widget.dart
│   │           ├── property_categories_widget.dart
│   │           └── popular_destinations_widget.dart
│   │
│   ├── search/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── search_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── search_filter_model.dart
│   │   │   │   └── search_result_model.dart
│   │   │   └── repositories/
│   │   │       └── search_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── search_filter.dart
│   │   │   │   └── search_result.dart
│   │   │   ├── repositories/
│   │   │   │   └── search_repository.dart
│   │   │   └── usecases/
│   │   │       ├── search_properties_usecase.dart
│   │   │       ├── get_search_filters_usecase.dart
│   │   │       └── get_search_suggestions_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── search_bloc.dart
│   │       │   ├── search_event.dart
│   │       │   └── search_state.dart
│   │       ├── pages/
│   │       │   ├── search_page.dart
│   │       │   ├── search_filters_page.dart
│   │       │   └── search_results_page.dart
│   │       └── widgets/
│   │           ├── search_input_widget.dart
│   │           ├── filter_chips_widget.dart
│   │           ├── sort_options_widget.dart
│   │           ├── price_range_slider_widget.dart
│   │           └── search_result_card_widget.dart
│   │
│   ├── property/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── property_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── property_detail_model.dart
│   │   │   │   ├── unit_model.dart
│   │   │   │   ├── amenity_model.dart
│   │   │   │   └── review_model.dart
│   │   │   └── repositories/
│   │   │       └── property_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── property_detail.dart
│   │   │   │   ├── unit.dart
│   │   │   │   ├── amenity.dart
│   │   │   │   └── property_policy.dart
│   │   │   ├── repositories/
│   │   │   │   └── property_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_property_details_usecase.dart
│   │   │       ├── get_property_units_usecase.dart
│   │   │       ├── get_property_reviews_usecase.dart
│   │   │       └── add_to_favorites_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── property_bloc.dart
│   │       │   ├── property_event.dart
│   │       │   └── property_state.dart
│   │       ├── pages/
│   │       │   ├── property_details_page.dart
│   │       │   ├── property_gallery_page.dart
│   │       │   ├── property_reviews_page.dart
│   │       │   └── property_map_page.dart
│   │       └── widgets/
│   │           ├── property_header_widget.dart
│   │           ├── property_images_slider_widget.dart
│   │           ├── property_info_widget.dart
│   │           ├── amenities_grid_widget.dart
│   │           ├── units_list_widget.dart
│   │           ├── reviews_summary_widget.dart
│   │           ├── policies_widget.dart
│   │           └── location_map_widget.dart
│   │
│   ├── booking/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── booking_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── booking_model.dart
│   │   │   │   ├── booking_request_model.dart
│   │   │   │   └── payment_model.dart
│   │   │   └── repositories/
│   │   │       └── booking_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── booking.dart
│   │   │   │   ├── booking_request.dart
│   │   │   │   └── payment.dart
│   │   │   ├── repositories/
│   │   │   │   └── booking_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_booking_usecase.dart
│   │   │       ├── get_booking_details_usecase.dart
│   │   │       ├── cancel_booking_usecase.dart
│   │   │       ├── get_user_bookings_usecase.dart
│   │   │       └── check_availability_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── booking_bloc.dart
│   │       │   ├── booking_event.dart
│   │       │   └── booking_state.dart
│   │       ├── pages/
│   │       │   ├── booking_form_page.dart
│   │       │   ├── booking_summary_page.dart
│   │       │   ├── booking_payment_page.dart
│   │       │   ├── booking_confirmation_page.dart
│   │       │   ├── my_bookings_page.dart
│   │       │   └── booking_details_page.dart
│   │       └── widgets/
│   │       └── widgets/
│   │           ├── date_picker_widget.dart
│   │           ├── guest_selector_widget.dart
│   │           ├── services_selector_widget.dart
│   │           ├── price_breakdown_widget.dart
│   │           ├── booking_card_widget.dart
│   │           ├── booking_status_widget.dart
│   │           └── payment_methods_widget.dart
│   │
│   ├── payment/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── payment_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── payment_method_model.dart
│   │   │   │   └── transaction_model.dart
│   │   │   └── repositories/
│   │   │       └── payment_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── payment_method.dart
│   │   │   │   └── transaction.dart
│   │   │   ├── repositories/
│   │   │   │   └── payment_repository.dart
│   │   │   └── usecases/
│   │   │       ├── process_payment_usecase.dart
│   │   │       ├── get_payment_methods_usecase.dart
│   │   │       └── get_payment_history_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── payment_bloc.dart
│   │       │   ├── payment_event.dart
│   │       │   └── payment_state.dart
│   │       ├── pages/
│   │       │   ├── payment_methods_page.dart
│   │       │   ├── add_payment_method_page.dart
│   │       │   └── payment_history_page.dart
│   │       └── widgets/
│   │           ├── credit_card_form_widget.dart
│   │           ├── payment_method_card_widget.dart
│   │           └── transaction_item_widget.dart
│   │
│   ├── chat/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── chat_local_datasource.dart
│   │   │   │   └── chat_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── conversation_model.dart
│   │   │   │   ├── message_model.dart
│   │   │   │   └── attachment_model.dart
│   │   │   └── repositories/
│   │   │       └── chat_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── conversation.dart
│   │   │   │   ├── message.dart
│   │   │   │   └── attachment.dart
│   │   │   ├── repositories/
│   │   │   │   └── chat_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_conversations_usecase.dart
│   │   │       ├── get_messages_usecase.dart
│   │   │       ├── send_message_usecase.dart
│   │   │       ├── mark_as_read_usecase.dart
│   │   │       └── upload_attachment_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── chat_bloc.dart
│   │       │   ├── chat_event.dart
│   │       │   └── chat_state.dart
│   │       ├── pages/
│   │       │   ├── conversations_page.dart
│   │       │   ├── chat_page.dart
│   │       │   └── chat_settings_page.dart
│   │       └── widgets/
│   │           ├── conversation_item_widget.dart
│   │           ├── message_bubble_widget.dart
│   │           ├── message_input_widget.dart
│   │           ├── typing_indicator_widget.dart
│   │           ├── attachment_preview_widget.dart
│   │           └── reaction_picker_widget.dart
│   │
│   ├── notifications/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   ├── notification_local_datasource.dart
│   │   │   │   └── notification_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── notification_model.dart
│   │   │   └── repositories/
│   │   │       └── notification_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── notification.dart
│   │   │   ├── repositories/
│   │   │   │   └── notification_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_notifications_usecase.dart
│   │   │       ├── mark_as_read_usecase.dart
│   │   │       ├── dismiss_notification_usecase.dart
│   │   │       └── update_notification_settings_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── notification_bloc.dart
│   │       │   ├── notification_event.dart
│   │       │   └── notification_state.dart
│   │       ├── pages/
│   │       │   ├── notifications_page.dart
│   │       │   └── notification_settings_page.dart
│   │       └── widgets/
│   │           ├── notification_item_widget.dart
│   │           ├── notification_badge_widget.dart
│   │           └── notification_filter_widget.dart
│   │
│   ├── favorites/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── favorites_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   └── favorite_property_model.dart
│   │   │   └── repositories/
│   │   │       └── favorites_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   └── favorite_property.dart
│   │   │   ├── repositories/
│   │   │   │   └── favorites_repository.dart
│   │   │   └── usecases/
│   │   │       ├── get_favorites_usecase.dart
│   │   │       ├── add_to_favorites_usecase.dart
│   │   │       └── remove_from_favorites_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── favorites_bloc.dart
│   │       │   ├── favorites_event.dart
│   │       │   └── favorites_state.dart
│   │       ├── pages/
│   │       │   └── favorites_page.dart
│   │       └── widgets/
│   │           ├── favorite_property_card_widget.dart
│   │           └── favorite_button_widget.dart
│   │
│   ├── review/
│   │   ├── data/
│   │   │   ├── datasources/
│   │   │   │   └── review_remote_datasource.dart
│   │   │   ├── models/
│   │   │   │   ├── review_model.dart
│   │   │   │   └── review_image_model.dart
│   │   │   └── repositories/
│   │   │       └── review_repository_impl.dart
│   │   │
│   │   ├── domain/
│   │   │   ├── entities/
│   │   │   │   ├── review.dart
│   │   │   │   └── review_image.dart
│   │   │   ├── repositories/
│   │   │   │   └── review_repository.dart
│   │   │   └── usecases/
│   │   │       ├── create_review_usecase.dart
│   │   │       ├── get_property_reviews_usecase.dart
│   │   │       └── upload_review_images_usecase.dart
│   │   │
│   │   └── presentation/
│   │       ├── bloc/
│   │       │   ├── review_bloc.dart
│   │       │   ├── review_event.dart
│   │       │   └── review_state.dart
│   │       ├── pages/
│   │       │   ├── write_review_page.dart
│   │       │   └── reviews_list_page.dart
│   │       └── widgets/
│   │           ├── review_form_widget.dart
│   │           ├── review_card_widget.dart
│   │           ├── rating_selector_widget.dart
│   │           └── review_images_picker_widget.dart
│   │
│   └── settings/
│       ├── data/
│       │   ├── datasources/
│       │   │   └── settings_local_datasource.dart
│       │   ├── models/
│       │   │   └── app_settings_model.dart
│       │   └── repositories/
│       │       └── settings_repository_impl.dart
│       │
│       ├── domain/
│       │   ├── entities/
│       │   │   └── app_settings.dart
│       │   ├── repositories/
│       │   │   └── settings_repository.dart
│       │   └── usecases/
│       │       ├── get_settings_usecase.dart
│       │       ├── update_language_usecase.dart
│       │       └── update_theme_usecase.dart
│       │
│       └── presentation/
│           ├── bloc/
│           │   ├── settings_bloc.dart
│           │   ├── settings_event.dart
│           │   └── settings_state.dart
│           ├── pages/
│           │   ├── settings_page.dart
│           │   ├── language_settings_page.dart
│           │   ├── privacy_policy_page.dart
│           │   └── about_page.dart
│           └── widgets/
│               ├── settings_item_widget.dart
│               ├── language_selector_widget.dart
│               └── theme_selector_widget.dart
│
├── routes/
│   ├── app_router.dart
│   ├── route_guards.dart
│   ├── dynamic_route_handler.dart  # ✨ جديد - معالج التوجيه الديناميكي
│   └── route_animations.dart
│
└── services/
    ├── local_storage_service.dart
    ├── location_service.dart
    ├── notification_service.dart
    ├── deep_link_service.dart
    ├── analytics_service.dart
    ├── crash_reporting_service.dart
    ├── cache_service.dart  # ✨ جديد - لتخزين القوالب محلياً
    ├── component_data_service.dart  # ✨ جديد - لجلب بيانات المكونات
    └── websocket_service.dart

# ملفات إضافية مهمة

test/
├── unit/
│   ├── auth/
│   ├── booking/
│   ├── property/
│   └── payment/
├── widget/
│   ├── auth_widgets_test.dart
│   ├── booking_widgets_test.dart
│   └── property_widgets_test.dart
└── integration/
    ├── auth_flow_test.dart
    ├── booking_flow_test.dart
    └── search_flow_test.dart

assets/
├── images/
│   ├── logo.png
│   ├── splash_screen.png
│   ├── banner_placeholder.png  # ✨ جديد
│   ├── carousel_placeholder.png  # ✨ جديد
│   └── placeholders/
├── icons/
│   ├── amenity_icons/
│   ├── category_icons/
│   └── component_icons/  # ✨ جديد
├── animations/
│   ├── loading.json
│   ├── success.json
│   ├── shimmer_loading.json  # ✨ جديد
│   └── component_transitions/  # ✨ جديد
└── fonts/
    ├── arabic_font.ttf
    └── english_font.ttf

# ملفات التكوين

pubspec.yaml
analysis_options.yaml
.env
.env.production