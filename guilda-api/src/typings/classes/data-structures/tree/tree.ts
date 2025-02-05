import { TreeNode } from './tree-node';
import { TreeKey } from './tree-key';

interface InsertParams<T> {
  parentNodeKey: TreeKey;
  key: TreeKey;
  value: T;
  children?: TreeNode<T>[];
}

export class Tree<T> {
  private readonly root: TreeNode<T>;
  private totalItems = 0;

  constructor(key: TreeKey, value: T) {
    this.root = new TreeNode<T>({ key, value, children: [] });
    this.increment();
  }

  private increment() {
    this.totalItems++;
  }

  get length() {
    return this.totalItems;
  }

  *preOrderTraversal(node = this.root): IterableIterator<TreeNode<T>> {
    yield node;
    if (node.children.length) {
      for (const child of node.children) {
        yield* this.preOrderTraversal(child);
      }
    }
  }

  *postOrderTraversal(node = this.root) {
    if (node.children.length) {
      for (const child of node.children) {
        yield* this.postOrderTraversal(child);
      }
    }
    yield node;
  }

  insert(node: InsertParams<T>): TreeNode<T> | null {
    const { parentNodeKey, key, value, children } = node;
    const parentNode = this.find(parentNodeKey);
    const treeNode = new TreeNode<T>({
      key,
      value,
      parent: parentNode,
      children,
    });
    parentNode?.children.push(treeNode);
    this.increment();
    return treeNode;
  }

  find(key: TreeKey): TreeNode<T> {
    for (const node of this.preOrderTraversal()) {
      if (node.key === key) return node;
    }
    return undefined;
  }

  getRoot() {
    return this.root;
  }

  toJson(node = this.root): any {
    const { key, value, children } = node;
    return {
      key,
      value,
      children: children.map((child) => this.toJson(child)),
    };
  }
}
