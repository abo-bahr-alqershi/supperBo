import 'package:flutter/material.dart';
import '../../../../core/utils/color_extensions.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';

class PropertyMapPage extends StatefulWidget {
  final String propertyId;
  final String propertyName;
  final double latitude;
  final double longitude;
  final String address;

  const PropertyMapPage({
    super.key,
    required this.propertyId,
    required this.propertyName,
    required this.latitude,
    required this.longitude,
    required this.address,
  });

  @override
  State<PropertyMapPage> createState() => _PropertyMapPageState();
}

class _PropertyMapPageState extends State<PropertyMapPage> {
  GoogleMapController? _mapController;
  MapType _currentMapType = MapType.normal;
  final Set<Marker> _markers = {};
  final Set<Circle> _circles = {};
  bool _showNearbyPlaces = false;
  String _selectedCategory = 'all';

  final List<NearbyPlace> _nearbyPlaces = [];

  @override
  void initState() {
    super.initState();
    _initializeMarkers();
  }

  @override
  void dispose() {
    _mapController?.dispose();
    super.dispose();
  }

  void _initializeMarkers() {
    _markers.add(
      Marker(
        markerId: const MarkerId('property'),
        position: LatLng(widget.latitude, widget.longitude),
        infoWindow: InfoWindow(
          title: widget.propertyName,
          snippet: widget.address,
        ),
        icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueRed),
      ),
    );

    _circles.add(
      Circle(
        circleId: const CircleId('property_radius'),
        center: LatLng(widget.latitude, widget.longitude),
        radius: 500,
        fillColor: AppColors.primary.withValues(alpha: 0.1),
        strokeColor: AppColors.primary.withValues(alpha: 0.3),
        strokeWidth: 2,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: _buildAppBar(),
      body: Stack(
        children: [
          _buildMap(),
          _buildMapControls(),
          if (_showNearbyPlaces) _buildNearbyPlacesPanel(),
        ],
      ),
    );
  }

  AppBar _buildAppBar() {
    return AppBar(
      backgroundColor: AppColors.surface,
      title: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'الموقع على الخريطة',
            style: AppTextStyles.heading3,
          ),
          Text(
            widget.propertyName,
            style: AppTextStyles.caption.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
      actions: [
        IconButton(
          onPressed: _toggleMapType,
          icon: Icon(
            _currentMapType == MapType.normal
                ? Icons.satellite_outlined
                : Icons.map_outlined,
          ),
        ),
      ],
    );
  }

  Widget _buildMap() {
    return GoogleMap(
      onMapCreated: (controller) {
        _mapController = controller;
        _setMapStyle(controller);
      },
      initialCameraPosition: CameraPosition(
        target: LatLng(widget.latitude, widget.longitude),
        zoom: 15,
      ),
      mapType: _currentMapType,
      markers: _markers,
      circles: _circles,
      myLocationEnabled: true,
      myLocationButtonEnabled: false,
      zoomControlsEnabled: false,
      mapToolbarEnabled: false,
      compassEnabled: true,
    );
  }

  Widget _buildMapControls() {
    return Positioned(
      right: AppDimensions.paddingMedium,
      bottom: AppDimensions.paddingMedium + 80,
      child: Column(
        children: [
          _buildControlButton(
            icon: Icons.add,
            onPressed: _zoomIn,
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          _buildControlButton(
            icon: Icons.remove,
            onPressed: _zoomOut,
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildControlButton(
            icon: Icons.my_location,
            onPressed: _goToMyLocation,
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildControlButton(
            icon: Icons.place_outlined,
            onPressed: () {
              setState(() {
                _showNearbyPlaces = !_showNearbyPlaces;
              });
              if (_showNearbyPlaces) {
                _loadNearbyPlaces();
              }
            },
            isActive: _showNearbyPlaces,
          ),
        ],
      ),
    );
  }

  Widget _buildControlButton({
    required IconData icon,
    required VoidCallback onPressed,
    bool isActive = false,
  }) {
    return Container(
      width: 48,
      height: 48,
      decoration: BoxDecoration(
        color: isActive ? AppColors.primary : AppColors.surface,
        shape: BoxShape.circle,
        boxShadow: const [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurMedium,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: IconButton(
        onPressed: onPressed,
        icon: Icon(
          icon,
          color: isActive ? AppColors.white : AppColors.textPrimary,
        ),
      ),
    );
  }

  Widget _buildNearbyPlacesPanel() {
    return Positioned(
      bottom: 0,
      left: 0,
      right: 0,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 300),
        height: 320,
        decoration: const BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.vertical(
            top: Radius.circular(AppDimensions.borderRadiusXl),
          ),
          boxShadow: [
            BoxShadow(
              color: AppColors.shadow,
              blurRadius: AppDimensions.blurLarge,
              offset: Offset(0, -2),
            ),
          ],
        ),
        child: Column(
          children: [
            _buildPanelHeader(),
            _buildCategoryFilter(),
            Expanded(
              child: _buildPlacesList(),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPanelHeader() {
    return Container(
      padding: const EdgeInsets.all(AppDimensions.paddingMedium),
      child: Column(
        children: [
          Container(
            width: 40,
            height: 4,
            decoration: BoxDecoration(
              color: AppColors.divider,
              borderRadius: BorderRadius.circular(2),
            ),
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                'الأماكن القريبة',
                style: AppTextStyles.heading3.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              IconButton(
                onPressed: () {
                  setState(() {
                    _showNearbyPlaces = false;
                  });
                },
                icon: const Icon(Icons.close),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildCategoryFilter() {
    final categories = [
      {'id': 'all', 'name': 'الكل', 'icon': Icons.apps},
      {'id': 'restaurant', 'name': 'مطاعم', 'icon': Icons.restaurant},
      {'id': 'cafe', 'name': 'مقاهي', 'icon': Icons.coffee},
      {'id': 'shopping', 'name': 'تسوق', 'icon': Icons.shopping_bag},
      {'id': 'hospital', 'name': 'مستشفيات', 'icon': Icons.local_hospital},
      {'id': 'atm', 'name': 'صراف آلي', 'icon': Icons.atm},
    ];

    return Container(
      height: 50,
      padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: categories.length,
        separatorBuilder: (context, index) => const SizedBox(
          width: AppDimensions.spacingSm,
        ),
        itemBuilder: (context, index) {
          final category = categories[index];
          final isSelected = _selectedCategory == category['id'];

          return FilterChip(
            selected: isSelected,
            onSelected: (selected) {
              setState(() {
                _selectedCategory = category['id'] as String;
              });
              _filterPlaces();
            },
            avatar: Icon(
              category['icon'] as IconData,
              size: 18,
              color: isSelected ? AppColors.primary : AppColors.textSecondary,
            ),
            label: Text(category['name'] as String),
            selectedColor: AppColors.primary.withValues(alpha: 0.1),
            backgroundColor: AppColors.surface,
            side: BorderSide(
              color: isSelected ? AppColors.primary : AppColors.border,
            ),
            labelStyle: AppTextStyles.bodySmall.copyWith(
              color: isSelected ? AppColors.primary : AppColors.textSecondary,
            ),
          );
        },
      ),
    );
  }

  Widget _buildPlacesList() {
    if (_nearbyPlaces.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.explore_off,
              size: 48,
              color: AppColors.textSecondary.withValues(alpha: 0.5),
            ),
            const SizedBox(height: AppDimensions.spacingMd),
            Text(
              'لا توجد أماكن قريبة',
              style: AppTextStyles.bodyMedium.copyWith(
                color: AppColors.textSecondary,
              ),
            ),
          ],
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.symmetric(horizontal: AppDimensions.paddingMedium),
      itemCount: _nearbyPlaces.length,
      separatorBuilder: (context, index) => const Divider(),
      itemBuilder: (context, index) {
        final place = _nearbyPlaces[index];
        return _buildPlaceItem(place);
      },
    );
  }

  Widget _buildPlaceItem(NearbyPlace place) {
    return InkWell(
      onTap: () => _showPlaceOnMap(place),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: AppDimensions.paddingSmall),
        child: Row(
          children: [
            Container(
              width: 48,
              height: 48,
              decoration: BoxDecoration(
                color: _getCategoryColor(place.category).withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(AppDimensions.borderRadiusMd),
              ),
              child: Icon(
                _getCategoryIcon(place.category),
                color: _getCategoryColor(place.category),
              ),
            ),
            const SizedBox(width: AppDimensions.spacingMd),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    place.name,
                    style: AppTextStyles.bodyMedium.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: AppDimensions.spacingXs),
                  Row(
                    children: [
                      const Icon(
                        Icons.location_on_outlined,
                        size: 14,
                        color: AppColors.textSecondary,
                      ),
                      const SizedBox(width: AppDimensions.spacingXs),
                      Text(
                        '${place.distance.toStringAsFixed(1)} كم',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                      const SizedBox(width: AppDimensions.spacingMd),
                      const Icon(
                        Icons.directions_walk,
                        size: 14,
                        color: AppColors.textSecondary,
                      ),
                      const SizedBox(width: AppDimensions.spacingXs),
                      Text(
                        '${place.walkingTime} دقيقة',
                        style: AppTextStyles.caption.copyWith(
                          color: AppColors.textSecondary,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
            IconButton(
              onPressed: () => _getDirections(place),
              icon: const Icon(
                Icons.directions,
                color: AppColors.primary,
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _setMapStyle(GoogleMapController controller) {
    controller.setMapStyle('''
    [
      {
        "featureType": "poi.business",
        "elementType": "labels",
        "stylers": [{"visibility": "off"}]
      }
    ]
    ''');
  }

  void _toggleMapType() {
    setState(() {
      _currentMapType = _currentMapType == MapType.normal
          ? MapType.satellite
          : MapType.normal;
    });
  }

  void _zoomIn() {
    _mapController?.animateCamera(CameraUpdate.zoomIn());
  }

  void _zoomOut() {
    _mapController?.animateCamera(CameraUpdate.zoomOut());
  }

  void _goToMyLocation() {
    // Implement go to current location
  }

  void _loadNearbyPlaces() {
    // Simulate loading nearby places
    setState(() {
      _nearbyPlaces.clear();
      _nearbyPlaces.addAll([
        NearbyPlace(
          name: 'مطعم الشام',
          category: 'restaurant',
          distance: 0.3,
          walkingTime: 5,
          latitude: widget.latitude + 0.002,
          longitude: widget.longitude + 0.001,
        ),
        NearbyPlace(
          name: 'كافيه السعادة',
          category: 'cafe',
          distance: 0.5,
          walkingTime: 8,
          latitude: widget.latitude - 0.003,
          longitude: widget.longitude + 0.002,
        ),
        NearbyPlace(
          name: 'سوبر ماركت النجمة',
          category: 'shopping',
          distance: 0.8,
          walkingTime: 12,
          latitude: widget.latitude + 0.004,
          longitude: widget.longitude - 0.003,
        ),
      ]);
    });
  }

  void _filterPlaces() {
    // Filter places based on selected category
  }

  void _showPlaceOnMap(NearbyPlace place) {
    _mapController?.animateCamera(
      CameraUpdate.newLatLngZoom(
        LatLng(place.latitude, place.longitude),
        17,
      ),
    );

    setState(() {
      _markers.add(
        Marker(
          markerId: MarkerId(place.name),
          position: LatLng(place.latitude, place.longitude),
          infoWindow: InfoWindow(
            title: place.name,
            snippet: '${place.distance} كم',
          ),
          icon: BitmapDescriptor.defaultMarkerWithHue(
            BitmapDescriptor.hueBlue,
          ),
        ),
      );
    });
  }

  void _getDirections(NearbyPlace place) {
    // Open directions in maps app
  }

  IconData _getCategoryIcon(String category) {
    switch (category) {
      case 'restaurant':
        return Icons.restaurant;
      case 'cafe':
        return Icons.coffee;
      case 'shopping':
        return Icons.shopping_bag;
      case 'hospital':
        return Icons.local_hospital;
      case 'atm':
        return Icons.atm;
      default:
        return Icons.place;
    }
  }

  Color _getCategoryColor(String category) {
    switch (category) {
      case 'restaurant':
        return AppColors.accent;
      case 'cafe':
        return Colors.brown;
      case 'shopping':
        return AppColors.secondary;
      case 'hospital':
        return AppColors.error;
      case 'atm':
        return AppColors.success;
      default:
        return AppColors.primary;
    }
  }
}

class NearbyPlace {
  final String name;
  final String category;
  final double distance;
  final int walkingTime;
  final double latitude;
  final double longitude;

  NearbyPlace({
    required this.name,
    required this.category,
    required this.distance,
    required this.walkingTime,
    required this.latitude,
    required this.longitude,
  });
}