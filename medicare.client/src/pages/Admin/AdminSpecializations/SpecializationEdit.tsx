import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { FaUserMd, FaAlignLeft, FaSave, FaArrowLeft } from "react-icons/fa";
import { getAdminSpecialization, updateAdminSpecialization, createAdminSpecialization, getAdminSpecializations } from "../../../services/specializationsService";
import type { Specialization } from "../../../interfaces/specialization.types";

export default function SpecializationEdit() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const isEdit = !!id && !isNaN(Number(id));

    const [specialization, setSpecialization] = useState<Specialization>({
        id: 0,
        specializationName: "",
        specializationHighlight: "",
        specializationDescription: ""
    });
    const [loading, setLoading] = useState(isEdit);
    const [errors, setErrors] = useState<{ specializationName?: string }>({});
    const [existingNames, setExistingNames] = useState<string[]>([]);

    useEffect(() => {
        getAdminSpecializations()
            .then(data => setExistingNames(data.map((s: Specialization) => s.specializationName.toLowerCase())))
            .catch(err => console.error(err));

        if (!isEdit) { setLoading(false); return; }
        (async () => {
            try {
                const data = await getAdminSpecialization(Number(id));
                setSpecialization(data);
            } catch (err) {
                console.error(err);
                alert("Failed to load specialization");
                navigate("/admin/specializations");
            } finally {
                setLoading(false);
            }
        })();
    }, [id]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
        setSpecialization(prev => ({ ...prev, [e.target.name]: e.target.value }));

    function validate(): boolean {
        const newErrors: { specializationName?: string } = {};
        const name = specialization.specializationName.trim().toLowerCase();

        if (!name) {
            newErrors.specializationName = "Specialization name is required.";
        } else if (existingNames.includes(name) && (!isEdit || name !== specialization.specializationName.toLowerCase())) {
            newErrors.specializationName = "Specialization name already exists. Please choose a different name.";
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!validate()) return;

        try {
            if (isEdit) {
                await updateAdminSpecialization(specialization.id, specialization);
                alert("Specialization updated");
            } else {
                await createAdminSpecialization({
                    specializationName: specialization.specializationName,
                    specializationHighlight: specialization.specializationHighlight,
                    specializationDescription: specialization.specializationDescription
                });
                alert("Specialization created");
            }
            navigate("/admin/specializations");
        } catch (err: any) {
            console.error(err);
            alert("Error saving specialization: " + (err.message ?? ""));
        }
    };

    if (loading) return <p>Loading...</p>;

    return (
        <div className="container py-4">
            <div className="card shadow-lg p-4">
                <h2 className="mb-4 text-center text-primary">{isEdit ? "Edit Specialization" : "Add Specialization"}</h2>
                <form onSubmit={handleSubmit}>
                    <div className="row g-3">
                        <div className="col-md-6">
                            <label className="form-label"><FaUserMd className="me-2" />Name</label>
                            <input
                                type="text"
                                name="specializationName"
                                value={specialization.specializationName}
                                onChange={handleChange}
                                className={`form-control ${errors.specializationName ? "is-invalid" : ""}`}
                                required
                            />
                            {errors.specializationName && <div className="invalid-feedback">{errors.specializationName}</div>}
                        </div>
                        <div className="col-md-6">
                            <label className="form-label"><FaAlignLeft className="me-2" />Highlight</label>
                            <input
                                type="text"
                                name="specializationHighlight"
                                value={specialization.specializationHighlight ?? ""}
                                onChange={handleChange}
                                className="form-control"
                            />
                        </div>
                        <div className="col-md-12">
                            <label className="form-label"><FaAlignLeft className="me-2" />Description</label>
                            <textarea
                                name="specializationDescription"
                                value={specialization.specializationDescription ?? ""}
                                onChange={handleChange}
                                className="form-control"
                                rows={4}
                            />
                        </div>
                    </div>

                    <div className="d-flex justify-content-between mt-4">
                        <button type="button" className="btn btn-outline-secondary" onClick={() => navigate("/admin/specializations")}>
                            <FaArrowLeft className="me-2" /> Back
                        </button>
                        <button type="submit" className="btn btn-primary">
                            <FaSave className="me-2" /> Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
