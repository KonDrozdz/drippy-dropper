export interface File {
  fileId: string;
  name: string;
  size: number;
  path: string;
  contentType: string;
  createdAt: string;
}
export interface FolderFiles{
  fileId: string,
  name: string,
  size: 0,
  contentType: string,
  createdAt: string
};


export interface Folder {
  folderId: string;
  name: string;
  files?: FolderFiles[];
  parentFolderId?: string;
  ownerId: string;
}
