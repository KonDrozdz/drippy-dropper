export interface File {
  id: string;
  name: string;
  url: string;
}

export interface Folder {
  id: string;
  name: string;
  subfolders?: Folder[];
  files?: File[];
}
