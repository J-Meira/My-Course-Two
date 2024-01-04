public record ProductDTO(
  string Code,
  string Name,
  string? Description,
  int CategoryId,
  List<string> Tags
);
