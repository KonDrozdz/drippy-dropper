import React, { useState } from "react";
import { Box, Typography, List, ListItem, ListItemText, ListItemAvatar, Avatar, IconButton, Collapse, Drawer } from "@mui/material";
import FolderIcon from "@mui/icons-material/Folder";
import DeleteIcon from "@mui/icons-material/Delete";
import ExpandLess from "@mui/icons-material/ExpandLess";
import ExpandMore from "@mui/icons-material/ExpandMore";
import FileIcon from "./FileIcon"; // Import the FileIcon component

interface File {
  id: string;
  name: string;
  url: string;
}

interface Folder {
  id: string;
  name: string;
  subfolders?: Folder[];
  files?: File[];
}

interface Item {
  id: string;
  name: string;
  type: "file" | "folder";
  url?: string;
  subfolders?: Folder[];
  files?: File[];
}

interface NestedFileListProps {
  items: Item[];
  handleDeleteFile: (fileId: string) => void;
}

const NestedFileList: React.FC<NestedFileListProps> = ({ items, handleDeleteFile }) => {
  const [openFolders, setOpenFolders] = useState<Set<string>>(new Set());
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const handleFolderClick = (folderId: string) => {
    const newOpenFolders = new Set(openFolders);
    if (newOpenFolders.has(folderId)) {
      newOpenFolders.delete(folderId);
    } else {
      newOpenFolders.add(folderId);
    }
    setOpenFolders(newOpenFolders);
  };

  const handleFileClick = (file: File) => {
    setSelectedFile(file);
  };

  const renderFolder = (folder: Folder) => {
    const isOpen = openFolders.has(folder.id);

    return (
      <div key={folder.id}>
        <ListItem component="button" onClick={() => handleFolderClick(folder.id)}>
          <ListItemAvatar>
            <Avatar>
              <FolderIcon />
            </Avatar>
          </ListItemAvatar>
          <ListItemText primary={folder.name} />
          {isOpen ? <ExpandLess /> : <ExpandMore />}
        </ListItem>
        <Collapse in={isOpen} timeout="auto" unmountOnExit>
          <List component="div" disablePadding>
            {folder.subfolders?.map(renderFolder)}
            {folder.files?.map((file) => (
              <ListItem key={file.id} sx={{ pl: 4 }} onClick={() => handleFileClick(file)}>
                <ListItemAvatar>
                  <Avatar>
                    <FileIcon fileName={file.name} />
                  </Avatar>
                </ListItemAvatar>
                <ListItemText
                  primary={file.name}
                  secondary={<a href={file.url} target="_blank" rel="noopener noreferrer">Zobacz plik</a>}
                />
                <IconButton edge="end" onClick={() => handleDeleteFile(file.id)} sx={{ color: 'red' }}>
                  <DeleteIcon />
                </IconButton>
              </ListItem>
            ))}
          </List>
        </Collapse>
      </div>
    );
  };

  const renderItem = (item: Item) => {
    if (item.type === "folder") {
      return renderFolder(item as Folder);
    } else {
      return (
        <ListItem key={item.id} component="button" onClick={() => handleFileClick(item as File)}>
          <ListItemAvatar>
            <Avatar>
              <FileIcon fileName={item.name} />
            </Avatar>
          </ListItemAvatar>
          <ListItemText
            primary={item.name}
            secondary={<a href={item.url} target="_blank" rel="noopener noreferrer">Zobacz plik</a>}
          />
          <IconButton edge="end" onClick={() => handleDeleteFile(item.id)} sx={{ color: 'red' }}>
            <DeleteIcon />
          </IconButton>
        </ListItem>
      );
    }
  };

  const renderFilePreview = (file: File) => {
    const fileExtension = file.name.split('.').pop()?.toLowerCase();
    switch (fileExtension) {
      case 'jpg':
      case 'jpeg':
      case 'png':
      case 'gif':
        return <img src={file.url} alt={file.name} style={{ width: '100%' }} />;
      case 'pdf':
        return <iframe src={file.url} width="100%" height="400px" title={file.name} />;
      case 'txt':
        return <iframe src={file.url} width="100%" height="400px" title={file.name} />;
      case 'xlsx':
      case 'xls':
        return <iframe src={`https://view.officeapps.live.com/op/embed.aspx?src=${file.url}`} width="100%" height="400px" title={file.name} />;
      default:
        return <Typography>Nieobsługiwany format pliku</Typography>;
    }
  };

  return (
    <Box sx={{ padding: 4 }}>
      <Typography variant="h4" mb={2}>
        Pliki i Foldery
      </Typography>
      <List sx={{ width: '100%', maxWidth: 360, color: 'white' }}>
        {items.map(renderItem)}
      </List>
      <Drawer
        anchor="right"
        open={Boolean(selectedFile)}
        onClose={() => setSelectedFile(null)}
        PaperProps={{ sx: { bgcolor: 'gray' } }}
      >
        <Box sx={{ width: 300, padding: 2 }}>
          {selectedFile && (
            <>
              <Typography variant="h6">{selectedFile.name}</Typography>
              {renderFilePreview(selectedFile)}
            </>
          )}
        </Box>
      </Drawer>
    </Box>
  );
};

export default NestedFileList;