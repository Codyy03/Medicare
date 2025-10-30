export default function NotFoundPage() {
    return (
        <div className="text-center d-flex flex-column min-vh-100 justify-content-center">
            <h1 className="text-3xl font-bold text-red-600">Page youre looking for doesn't exist</h1>
            <h2 className="text-3xl font-bold text-red-600"></h2>
            <p className="mt-4">

                <h2 className="text-3xl font-bold text-red-600">
                    <a href="/" className="text-blue-500 underline mt-2 inline-block">Get back home</a>
                </h2>
                

            </p>
        </div>
    );
}
