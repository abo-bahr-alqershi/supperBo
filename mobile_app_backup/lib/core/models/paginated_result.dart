import 'package:equatable/equatable.dart';

class PaginatedResult<T> extends Equatable {
  final List<T> items;
  final int pageNumber;
  final int pageSize;
  final int totalCount;
  final Object? metadata;

  const PaginatedResult({
    required this.items,
    required this.pageNumber,
    required this.pageSize,
    required this.totalCount,
    this.metadata,
  });

  int get totalPages => (totalCount / pageSize).ceil();
  bool get hasPreviousPage => pageNumber > 1;
  bool get hasNextPage => pageNumber < totalPages;
  int? get previousPageNumber => hasPreviousPage ? pageNumber - 1 : null;
  int? get nextPageNumber => hasNextPage ? pageNumber + 1 : null;
  int get startIndex => (pageNumber - 1) * pageSize + 1;
  int get endIndex => startIndex + items.length - 1;

  factory PaginatedResult.empty({
    int pageNumber = 1,
    int pageSize = 10,
  }) {
    return PaginatedResult<T>(
      items: [],
      pageNumber: pageNumber,
      pageSize: pageSize,
      totalCount: 0,
    );
  }

  factory PaginatedResult.fromJson(
    Map<String, dynamic> json,
    T Function(Map<String, dynamic>) fromJsonT,
  ) {
    return PaginatedResult<T>(
      items: (json['items'] as List?)
              ?.map((item) => fromJsonT(item))
              .toList() ??
          [],
      pageNumber: json['pageNumber'] ?? 1,
      pageSize: json['pageSize'] ?? 10,
      totalCount: json['totalCount'] ?? 0,
      metadata: json['metadata'],
    );
  }

  Map<String, dynamic> toJson(Map<String, dynamic> Function(T) toJsonT) {
    return {
      'items': items.map((item) => toJsonT(item)).toList(),
      'pageNumber': pageNumber,
      'pageSize': pageSize,
      'totalCount': totalCount,
      'totalPages': totalPages,
      'hasPreviousPage': hasPreviousPage,
      'hasNextPage': hasNextPage,
      'previousPageNumber': previousPageNumber,
      'nextPageNumber': nextPageNumber,
      'startIndex': startIndex,
      'endIndex': endIndex,
      'metadata': metadata,
    };
  }

  @override
  List<Object?> get props => [
        items,
        pageNumber,
        pageSize,
        totalCount,
        metadata,
      ];
}