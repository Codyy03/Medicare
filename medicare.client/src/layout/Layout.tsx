import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import CarouselSection from "../components/CarouselSection";
import Tests from "../components/Tests";
import News from "../components/News";

export default function Layout() {
    return (
        <>
            <Header />
            <CarouselSection />
            <Tests />
            <News/>
            <main className="container mt-4">
                <Outlet />
            </main>
        </>
    );
}
