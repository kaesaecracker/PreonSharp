### ToDos for NCBI Taxonomy data source

- [x] load names.dmp as entities
- [x] load nodes.dmp as entity relations
- [x] load images.dmp and annotate entities with images for display in frontend
- [ ] load division.dmp and annotate entities with those
- [x] merged.dmp may be used to merge entities before loading (or may contain ids not used anymore)
      --> merged nodes are note listed anymore so can be ignored
- [ ] filter tags from names.dmp (e.g. "authority"), or separate those out somehow
- [ ] specify both names from names.dmp instead of one

### ToDos for NCBI gene_info.tsv data source

- [ ] LOCUS Tag might be an additional ID and should be treated as such
- [ ] "synonyms" might be a list sometimes and should be split 