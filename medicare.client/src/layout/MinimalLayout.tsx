import Header from "../components/Header";
import Footer from "../components/Footer";

export default function MinimalLayout({ children }: { children: React.ReactNode }) {
    return (
        <>
            <Header />
            <main className="container mt-4">{children}</main>
            <Footer />
        </>
    );
}
