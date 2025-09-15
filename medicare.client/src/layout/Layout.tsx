import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import CarouselSection from "../components/CarouselSection";
import Tests from "../components/Tests";

export default function Layout() {
    return (
        <>
            <Header />
            <CarouselSection />
            <Tests/>
            <main className="container mt-4">
                <Outlet />
            </main>
        </>
    );
}
