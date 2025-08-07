import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_dimensions.dart';
import '../../../../core/theme/app_text_styles.dart';
import '../../domain/entities/search_result.dart';
import '../widgets/search_result_card_widget.dart';

class SearchResultsMapPage extends StatefulWidget {
  final List<SearchResult> results;
  final VoidCallback? onBackToList;

  const SearchResultsMapPage({
    super.key,
    required this.results,
    this.onBackToList,
  });

  @override
  State<SearchResultsMapPage> createState() => _SearchResultsMapPageState();
}

class _SearchResultsMapPageState extends State<SearchResultsMapPage> {
  GoogleMapController? _mapController;
  Set<Marker> _markers = {};
  String? _selectedMarkerId;
  final PageController _pageController = PageController(viewportFraction: 0.9);
  
  // Yemen coordinates (Sana'a)
  static const LatLng _defaultLocation = LatLng(15.3694, 44.1910);

  @override
  void initState() {
    super.initState();
    _initializeMarkers();
  }

  @override
  void dispose() {
    _mapController?.dispose();
    _pageController.dispose();
    super.dispose();
  }

  void _initializeMarkers() {
    _markers = widget.results.map((result) {
      return Marker(
        markerId: MarkerId(result.id),
        position: LatLng(result.latitude, result.longitude),
        onTap: () => _onMarkerTapped(result),
        icon: BitmapDescriptor.defaultMarkerWithHue(
          _selectedMarkerId == result.id
              ? BitmapDescriptor.hueGreen
              : BitmapDescriptor.hueRed,
        ),
        infoWindow: InfoWindow(
          title: result.name,
          snippet: '${result.minPrice} ${result.currency}',
        ),
      );
    }).toSet();
  }

  void _onMarkerTapped(SearchResult result) {
    setState(() {
      _selectedMarkerId = result.id;
    });
    
    final index = widget.results.indexWhere((r) => r.id == result.id);
    if (index != -1) {
      _pageController.animateToPage(
        index,
        duration: const Duration(milliseconds: 300),
        curve: Curves.easeInOut,
      );
    }
  }

  void _onPropertyCardTapped(SearchResult result) {
    setState(() {
      _selectedMarkerId = result.id;
      _initializeMarkers();
    });
    
    _mapController?.animateCamera(
      CameraUpdate.newLatLngZoom(
        LatLng(result.latitude, result.longitude),
        15,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          _buildMap(),
          _buildTopBar(),
          _buildPropertyCards(),
          _buildMapControls(),
        ],
      ),
    );
  }

  Widget _buildMap() {
    return GoogleMap(
      onMapCreated: (controller) {
        _mapController = controller;
        _setMapStyle(controller);
      },
      initialCameraPosition: CameraPosition(
        target: _calculateCenter(),
        zoom: 12,
      ),
      markers: _markers,
      myLocationEnabled: true,
      myLocationButtonEnabled: false,
      zoomControlsEnabled: false,
      mapToolbarEnabled: false,
      compassEnabled: false,
    );
  }

  Widget _buildTopBar() {
    return Positioned(
      top: 0,
      left: 0,
      right: 0,
      child: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [
              AppColors.black.withOpacity(0.7),
              AppColors.black.withOpacity(0.0),
            ],
          ),
        ),
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(AppDimensions.paddingMedium),
            child: Row(
              children: [
                _buildCircularButton(
                  icon: Icons.arrow_back_rounded,
                  onPressed: widget.onBackToList ?? () => Navigator.pop(context),
                ),
                const Spacer(),
                _buildCircularButton(
                  icon: Icons.layers_rounded,
                  onPressed: _showMapTypeSelector,
                ),
                const SizedBox(width: AppDimensions.spacingSm),
                _buildCircularButton(
                  icon: Icons.filter_list_rounded,
                  onPressed: _showFilterOptions,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildPropertyCards() {
    if (widget.results.isEmpty) {
      return const SizedBox.shrink();
    }

    return Positioned(
      bottom: MediaQuery.of(context).padding.bottom + AppDimensions.paddingMedium,
      left: 0,
      right: 0,
      height: 140,
      child: PageView.builder(
        controller: _pageController,
        onPageChanged: (index) {
          final result = widget.results[index];
          _onPropertyCardTapped(result);
        },
        itemCount: widget.results.length,
        itemBuilder: (context, index) {
          final result = widget.results[index];
          return Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: AppDimensions.paddingSmall,
            ),
            child: _MapPropertyCard(
              result: result,
              isSelected: _selectedMarkerId == result.id,
              onTap: () => _onPropertyCardTapped(result),
            ),
          );
        },
      ),
    );
  }

  Widget _buildMapControls() {
    return Positioned(
      right: AppDimensions.paddingMedium,
      bottom: 180,
      child: Column(
        children: [
          _buildCircularButton(
            icon: Icons.add_rounded,
            onPressed: _zoomIn,
            size: 40,
          ),
          const SizedBox(height: AppDimensions.spacingSm),
          _buildCircularButton(
            icon: Icons.remove_rounded,
            onPressed: _zoomOut,
            size: 40,
          ),
          const SizedBox(height: AppDimensions.spacingMd),
          _buildCircularButton(
            icon: Icons.my_location_rounded,
            onPressed: _goToMyLocation,
            size: 40,
          ),
        ],
      ),
    );
  }

  Widget _buildCircularButton({
    required IconData icon,
    required VoidCallback onPressed,
    double size = 48,
  }) {
    return Container(
      width: size,
      height: size,
      decoration: BoxDecoration(
        color: AppColors.surface,
        shape: BoxShape.circle,
        boxShadow: [
          BoxShadow(
            color: AppColors.shadow,
            blurRadius: AppDimensions.blurMedium,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: IconButton(
        onPressed: onPressed,
        icon: Icon(
          icon,
          size: size * 0.5,
          color: AppColors.textPrimary,
        ),
        padding: EdgeInsets.zero,
      ),
    );
  }

  LatLng _calculateCenter() {
    if (widget.results.isEmpty) {
      return _defaultLocation;
    }

    double sumLat = 0;
    double sumLng = 0;

    for (final result in widget.results) {
      sumLat += result.latitude;
      sumLng += result.longitude;
    }

    return LatLng(
      sumLat / widget.results.length,
      sumLng / widget.results.length,
    );
  }

  void _setMapStyle(GoogleMapController controller) {
    controller.setMapStyle('''
    [
      {
        "featureType": "poi",
        "elementType": "labels",
        "stylers": [{"visibility": "off"}]
      }
    ]
    ''');
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

  void _showMapTypeSelector() {
    showModalBottomSheet(
      context: context,
      backgroundColor: AppColors.surface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(
          top: Radius.circular(AppDimensions.borderRadiusLg),
        ),
      ),
      builder: (context) {
        return SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                margin: const EdgeInsets.only(top: AppDimensions.paddingSmall),
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: AppColors.divider,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Padding(
                padding: const EdgeInsets.all(AppDimensions.paddingMedium),
                child: Text(
                  'نوع الخريطة',
                  style: AppTextStyles.heading3,
                ),
              ),
              ListTile(
                leading: const Icon(Icons.map_outlined),
                title: const Text('عادية'),
                onTap: () {
                  Navigator.pop(context);
                  // Set normal map type
                },
              ),
              ListTile(
                leading: const Icon(Icons.satellite_outlined),
                title: const Text('قمر صناعي'),
                onTap: () {
                  Navigator.pop(context);
                  // Set satellite map type
                },
              ),
              ListTile(
                leading: const Icon(Icons.terrain_outlined),
                title: const Text('تضاريس'),
                onTap: () {
                  Navigator.pop(context);
                  // Set terrain map type
                },
              ),
            ],
          ),
        );
      },
    );
  }

  void _showFilterOptions() {
    // Show filter options
  }
}

class _MapPropertyCard extends StatelessWidget {
  final SearchResult result;
  final bool isSelected;
  final VoidCallback onTap;

  const _MapPropertyCard({
    required this.result,
    required this.isSelected,
    required this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: AnimatedContainer(
        duration: const Duration(milliseconds: 200),
        decoration: BoxDecoration(
          color: AppColors.surface,
          borderRadius: BorderRadius.circular(AppDimensions.borderRadiusLg),
          border: Border.all(
            color: isSelected ? AppColors.primary : AppColors.outline,
            width: isSelected ? 2 : 1,
          ),
          boxShadow: [
            BoxShadow(
              color: isSelected
                  ? AppColors.primary.withOpacity(0.3)
                  : AppColors.shadow,
              blurRadius: isSelected
                  ? AppDimensions.blurLarge
                  : AppDimensions.blurMedium,
              offset: const Offset(0, 2),
            ),
          ],
        ),
        child: Row(
          children: [
            Container(
              width: 120,
              height: double.infinity,
              decoration: BoxDecoration(
                borderRadius: const BorderRadius.horizontal(
                  right: Radius.circular(AppDimensions.borderRadiusLg),
                ),
                image: DecorationImage(
                  image: NetworkImage(result.mainImageUrl ?? ''),
                  fit: BoxFit.cover,
                ),
              ),
              child: Stack(
                children: [
                  if (result.isFeatured)
                    Positioned(
                      top: AppDimensions.paddingSmall,
                      right: AppDimensions.paddingSmall,
                      child: Container(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppDimensions.paddingSmall,
                          vertical: AppDimensions.paddingXSmall,
                        ),
                        decoration: BoxDecoration(
                          color: AppColors.accent,
                          borderRadius: BorderRadius.circular(
                            AppDimensions.borderRadiusXs,
                          ),
                        ),
                        child: Text(
                          'مميز',
                          style: AppTextStyles.overline.copyWith(
                            color: AppColors.white,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ),
                    ),
                ],
              ),
            ),
            Expanded(
              child: Padding(
                padding: const EdgeInsets.all(AppDimensions.paddingSmall),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: [
                    Text(
                      result.name,
                      style: AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                    ),
                    Row(
                      children: [
                        Icon(
                          Icons.location_on_outlined,
                          size: AppDimensions.iconXSmall,
                          color: AppColors.textSecondary,
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Expanded(
                          child: Text(
                            result.address,
                            style: AppTextStyles.caption.copyWith(
                              color: AppColors.textSecondary,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      ],
                    ),
                    Row(
                      children: [
                        Icon(
                          Icons.star_rounded,
                          size: AppDimensions.iconSmall,
                          color: AppColors.ratingStar,
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Text(
                          result.averageRating.toStringAsFixed(1),
                          style: AppTextStyles.caption.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Text(
                          '(${result.reviewsCount})',
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                        const Spacer(),
                        Text(
                          '${result.minPrice.toStringAsFixed(0)}',
                          style: AppTextStyles.priceSmall.copyWith(
                            color: AppColors.primary,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(width: AppDimensions.spacingXs),
                        Text(
                          result.currency,
                          style: AppTextStyles.caption.copyWith(
                            color: AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}