import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_text_styles.dart';

class LocationMapWidget extends StatefulWidget {
  final double latitude;
  final double longitude;
  final String propertyName;
  final String address;

  const LocationMapWidget({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.propertyName,
    required this.address,
  });

  @override
  State<LocationMapWidget> createState() => _LocationMapWidgetState();
}

class _LocationMapWidgetState extends State<LocationMapWidget> {
  GoogleMapController? _mapController;
  final Set<Marker> _markers = {};

  @override
  void initState() {
    super.initState();
    _initializeMarker();
  }

  void _initializeMarker() {
    _markers.add(
      Marker(
        markerId: const MarkerId('property'),
        position: LatLng(widget.latitude, widget.longitude),
        infoWindow: InfoWindow(
          title: widget.propertyName,
          snippet: widget.address,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  const Icon(
                    Icons.location_on,
                    color: AppColors.primary,
                    size: 24,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      widget.address,
                      style: AppTextStyles.bodyMedium.copyWith(
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  _buildLocationAction(
                    icon: Icons.directions,
                    label: 'الاتجاهات',
                    onTap: _openDirections,
                  ),
                  const SizedBox(width: 8),
                  _buildLocationAction(
                    icon: Icons.share_location,
                    label: 'مشاركة الموقع',
                    onTap: _shareLocation,
                  ),
                  const SizedBox(width: 8),
                  _buildLocationAction(
                    icon: Icons.map,
                    label: 'عرض بملء الشاشة',
                    onTap: _openFullScreenMap,
                  ),
                ],
              ),
            ],
          ),
        ),
        Container(
          height: 300,
          margin: const EdgeInsets.symmetric(horizontal: 16.0),
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(12.0),
            border: Border.all(
              color: AppColors.border,
              width: 1,
            ),
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(12.0),
            child: GoogleMap(
              initialCameraPosition: CameraPosition(
                target: LatLng(widget.latitude, widget.longitude),
                zoom: 15,
              ),
              markers: _markers,
              onMapCreated: (GoogleMapController controller) {
                _mapController = controller;
              },
              zoomControlsEnabled: false,
              mapToolbarEnabled: false,
              myLocationButtonEnabled: false,
              compassEnabled: true,
              mapType: MapType.normal,
            ),
          ),
        ),
        const SizedBox(height: 16),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16.0),
          child: _buildNearbyPlaces(),
        ),
      ],
    );
  }

  Widget _buildLocationAction({
    required IconData icon,
    required String label,
    required VoidCallback onTap,
  }) {
    return Expanded(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(8.0),
        child: Container(
          padding: const EdgeInsets.symmetric(vertical: 8.0),
          decoration: BoxDecoration(
            color: AppColors.inputBackground,
            borderRadius: BorderRadius.circular(8.0),
            border: Border.all(
              color: AppColors.border,
              width: 1,
            ),
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(
                icon,
                color: AppColors.primary,
                size: 20,
              ),
              const SizedBox(height: 4),
              Text(
                label,
                style: AppTextStyles.bodySmall.copyWith(
                  fontSize: 11,
                  color: AppColors.textSecondary,
                ),
                textAlign: TextAlign.center,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildNearbyPlaces() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          'الأماكن القريبة',
          style: AppTextStyles.heading3.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 16),
        _buildNearbyItem(
          icon: Icons.school,
          title: 'مدرسة الملك فهد',
          distance: '500 م',
        ),
        _buildNearbyItem(
          icon: Icons.local_hospital,
          title: 'مستشفى الملك عبدالعزيز',
          distance: '1.2 كم',
        ),
        _buildNearbyItem(
          icon: Icons.shopping_cart,
          title: 'سوبر ماركت العثيم',
          distance: '300 م',
        ),
        _buildNearbyItem(
          icon: Icons.mosque,
          title: 'مسجد الرحمن',
          distance: '200 م',
        ),
      ],
    );
  }

  Widget _buildNearbyItem({
    required IconData icon,
    required String title,
    required String distance,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 8.0),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(8.0),
            ),
            child: Icon(
              icon,
              size: 20,
              color: AppColors.primary,
            ),
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              title,
              style: AppTextStyles.bodyMedium,
            ),
          ),
          Text(
            distance,
            style: AppTextStyles.bodySmall.copyWith(
              color: AppColors.textSecondary,
            ),
          ),
        ],
      ),
    );
  }

  void _openDirections() {
    // يمكنك استخدام url_launcher لفتح تطبيق الخرائط
    // final url = 'https://www.google.com/maps/dir/?api=1&destination=${widget.latitude},${widget.longitude}';
    // launchUrl(Uri.parse(url));
    
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('فتح الاتجاهات...'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  void _shareLocation() {
    // يمكنك استخدام share_plus لمشاركة الموقع
    // Share.share('موقع ${widget.propertyName}: https://maps.google.com/?q=${widget.latitude},${widget.longitude}');
    
    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        content: Text('مشاركة الموقع...'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  void _openFullScreenMap() {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => FullScreenMapView(
          latitude: widget.latitude,
          longitude: widget.longitude,
          propertyName: widget.propertyName,
          address: widget.address,
        ),
      ),
    );
  }

  @override
  void dispose() {
    _mapController?.dispose();
    super.dispose();
  }
}

// شاشة الخريطة بملء الشاشة
class FullScreenMapView extends StatelessWidget {
  final double latitude;
  final double longitude;
  final String propertyName;
  final String address;

  const FullScreenMapView({
    super.key,
    required this.latitude,
    required this.longitude,
    required this.propertyName,
    required this.address,
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      extendBodyBehindAppBar: true,
      appBar: AppBar(
        backgroundColor: Colors.transparent,
        elevation: 0,
        leading: Container(
          margin: const EdgeInsets.all(8),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(8.0),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.1),
                blurRadius: 8,
                offset: const Offset(0, 2),
              ),
            ],
          ),
          child: IconButton(
            icon: const Icon(Icons.arrow_back, color: Colors.black),
            onPressed: () => Navigator.pop(context),
          ),
        ),
      ),
      body: GoogleMap(
        initialCameraPosition: CameraPosition(
          target: LatLng(latitude, longitude),
          zoom: 16,
        ),
        markers: {
          Marker(
            markerId: const MarkerId('property'),
            position: LatLng(latitude, longitude),
            infoWindow: InfoWindow(
              title: propertyName,
              snippet: address,
            ),
          ),
        },
        myLocationEnabled: true,
        myLocationButtonEnabled: true,
        zoomControlsEnabled: true,
        mapToolbarEnabled: true,
        compassEnabled: true,
      ),
    );
  }
}