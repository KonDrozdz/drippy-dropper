import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { TextField, Button, Box, Typography } from "@mui/material";


const Login: React.FC = () => {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      const response = await axios.post<{ token: string }>("http://localhost:5000/api/login", {
        email,
        password,
      });
      // Zapisz token
      localStorage.setItem("token", response.data.token);
      navigate("/dashboard");
    } catch (error) {
      console.error("Login failed:", error);
    } 
  };

  return (
    <Box sx={{ maxWidth: 400, margin: "auto", mt: 8, textAlign: "center" }}>
      <Typography variant="h5" mb={2}>
        Logowanie
      </Typography>
      <form onSubmit={handleLogin}>
        <TextField
          fullWidth
          label="Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          margin="normal"
          sx={{
            backgroundColor: "white", // Tło inputa na biało
            borderRadius: 1, // Zaokrąglone rogi
          }}
        />
        <TextField
          fullWidth
          label="Hasło"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          margin="normal"
          sx={{
            backgroundColor: "white", // Tło inputa na biało
            borderRadius: 1, // Zaokrąglone rogi
          }}
        />
        <Button variant="contained" color="primary" type="submit" fullWidth>
          Zaloguj się
        </Button>
      </form>
    </Box>
  );
};

export default Login;
