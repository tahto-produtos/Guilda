export interface Post {
  cod: number;
  isRepost: number;
  imageUser: string;
  nameUser: string;
  hierarchyUser: string;
  post: string;
  idPostReference: number;
  postReference: string;
  imageUserReference: string;
  nameReference: string;
  hierarchyuserReference: string;
  datePost: string;
  timeAgo: string;
  linkFile: string;
  linkFileReference: string;
  comments: number;
  expiredAt: string;
  visibility: string;
  allowComment: 0 | 1;
  highlight: number;
  sumReactions: number;
  canDeletePost: boolean;
  myReaction: {
    id: number;
    name: string;
    linkIcon: string;
    linkIconSelected: string;
    amount: number;
  };
  reactions: {
    id: number;
    name: string;
    linkIcon: string;
    linkIconSelected: string;
    amount: number;
  }[];
  files: {
    url: string;
  }[];
  filesReference: {
    url: string;
  }[];
}
