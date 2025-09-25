import { Outlet } from "react-router-dom";
import Header from "../components/Header/Header";
import Footer from "../components/Footer/Footer";

export default function MinimalLayout() {
    return (
        <>
            <Header />
            <main className="container mt-4">
                <Outlet />
            </main>
            <Footer />
        </>
    );
}
