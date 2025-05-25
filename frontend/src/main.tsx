import React from "react";
import ReactDOM from "react-dom/client";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import './index.css';
import { DarkModeProvider } from "./context/DarkModeContext"; 

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <DarkModeProvider>
      <QueryClientProvider client={queryClient}>
         <BrowserRouter>
           <App />
         </BrowserRouter>
      </QueryClientProvider>
    </DarkModeProvider>
  </React.StrictMode>
);

