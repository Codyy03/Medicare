import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./layout/Layout";
import PatientsPage from "./pages/PatientsPage";
import ClientPage from "./pages/ClientPanel";
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route path="patients" element={<PatientsPage />} />
                    <Route path="client" element={<ClientPage />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;