import type { TreeNode } from '@/types/tree';

export interface FilterResult {
    /** The filtered tree (copies of the matching branches, endpoints stay by reference). */
    nodes: TreeNode[];
    /** Ids of the nodes whose own name matched the search term. */
    matchedIds: Set<string>;
    /** Ids of every node that survived the filter - used to auto expand the result. */
    visibleIds: Set<string>;
}

function normalize(value: string): string {
    return value.toLocaleLowerCase();
}

/**
 * Filters the tree by node name (search as you type).
 *
 * Rules:
 *  - a node is kept when its own name contains the term OR when a descendant matches;
 *  - when a node matches, its complete subtree is kept so the context stays visible;
 *  - the parents of a match are kept so the path from the root stays intact.
 */
export function filterTree(nodes: TreeNode[], term: string): FilterResult {
    const trimmed = term.trim();

    if (trimmed.length === 0) {
        return { nodes, matchedIds: new Set(), visibleIds: new Set() };
    }

    const needle = normalize(trimmed);
    const matchedIds = new Set<string>();
    const visibleIds = new Set<string>();

    function walk(node: TreeNode): TreeNode | null {
        const selfMatches = normalize(node.name).includes(needle);

        if (selfMatches) {
            matchedIds.add(node.id);
        }

        // A matching node keeps its entire subtree.
        const children = selfMatches
            ? node.children
            : (node.children.map(walk).filter(Boolean) as TreeNode[]);

        if (!selfMatches && children.length === 0) {
            return null;
        }

        visibleIds.add(node.id);

        if (selfMatches) {
            for (const id of collect(node.children)) {
                visibleIds.add(id);
            }
        }

        return { ...node, children };
    }

    function collect(list: TreeNode[], target: string[] = []): string[] {
        for (const child of list) {
            target.push(child.id);
            collect(child.children, target);
        }

        return target;
    }

    const filtered = nodes.map(walk).filter(Boolean) as TreeNode[];

    return { nodes: filtered, matchedIds, visibleIds };
}

/** Splits a text into the parts before / inside / after the search hit. */
export interface HighlightPart {
    text: string;
    hit: boolean;
}

export function highlightParts(text: string, term: string): HighlightPart[] {
    const trimmed = term.trim();

    if (trimmed.length === 0) {
        return [{ text, hit: false }];
    }

    const haystack = normalize(text);
    const needle = normalize(trimmed);
    const parts: HighlightPart[] = [];

    let index = 0;

    for (;;) {
        const hitIndex = haystack.indexOf(needle, index);

        if (hitIndex < 0) {
            break;
        }

        if (hitIndex > index) {
            parts.push({ text: text.slice(index, hitIndex), hit: false });
        }

        parts.push({ text: text.slice(hitIndex, hitIndex + needle.length), hit: true });
        index = hitIndex + needle.length;
    }

    if (index < text.length) {
        parts.push({ text: text.slice(index), hit: false });
    }

    return parts;
}
