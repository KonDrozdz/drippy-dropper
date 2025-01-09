import React from "react";
import FolderIcon from "@mui/icons-material/Folder";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import TextFieldsIcon from "@mui/icons-material/TextFields";
import ImageIcon from "@mui/icons-material/Image";
import InsertDriveFileIcon from "@mui/icons-material/InsertDriveFile";

// Komponent, który zwraca odpowiednią ikonę w zależności od rozszerzenia pliku
const FileIcon: React.FC<{ fileName: string }> = ({ fileName }) => {
  const extension = fileName.split(".").pop()?.toLowerCase();

  switch (extension) {
    case "pdf":
      return <PictureAsPdfIcon />;
    case "txt":
      return <TextFieldsIcon />;
    case "jpg":
    case "jpeg":
    case "png":
      return <ImageIcon />;
    case "xlsx":
    case "xls":
      return <InsertDriveFileIcon />;
    default:
      return <FolderIcon />;
  }
};

export default FileIcon;
