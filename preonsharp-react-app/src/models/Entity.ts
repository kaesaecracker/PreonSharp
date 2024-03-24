import {Guid} from './Guid.ts';

export type EntitySource = {
  readonly idNamespace: string;
  readonly sourceId: string;
};

export type EntityTag = {
  readonly kind: string;
  readonly value: string;
}

export type EntityRelation = {
  readonly kind: string;
  readonly other: Guid;
}

export type Entity = {
  readonly id: Guid;
  readonly sources: EntitySource[];
  readonly tags: EntityTag[];
  readonly names: EntityTag[];
  readonly relations: EntityRelation[];
}
