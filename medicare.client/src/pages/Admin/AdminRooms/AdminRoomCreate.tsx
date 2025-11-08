import React, { useState , useEffect} from "react";
import { useNavigate } from "react-router-dom";
import { createAdminRoom } from "../../../services/roomsService";
import type { RoomsDto } from "../../../interfaces/rooms.types";
import { getSpecializationNames } from "../../../services/specializationsService";
import type { SpecializationNameDto } from "../../../interfaces/specialization.types";
import Select from "react-select";
export default function AdminPatientsCreate() {
    const [form, setForm] = useState<RoomsDto>({
        roomNumber: null,
        roomType: "",
        specializations: []
    });

    const [errors, setErrors] = useState<{ [key: string]: string }>({});
    const [validated, setValidated] = useState(false);
    const [specializations, setSpecializations] = useState<SpecializationNameDto[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        getSpecializationNames()
            .then(data => setSpecializations(data))
            .catch(err => console.error(err))
            .finally(() => setLoading(false));
    }, []);

    function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
        const { name, value } = e.target;
        const updatedForm = { ...form, [name]: value };
        setForm(prev => ({ ...prev, [name]: value }));

        if (validated) {
            validateForm(updatedForm);
        }
    };

    function validateForm(vForm: RoomsDto) {
        const newErrors: { [key: string]: string } = {};
        let ok = true;

        if (isNaN(Number(vForm.roomNumber))) {
            newErrors["RoomNumber"] = "Must be number";
            ok = false;
        } else if (Number(vForm.roomNumber) < 1) {
            newErrors["RoomNumber"] = "Number must be positive";
            ok = false;
        }

        if (!vForm.roomType) {
            newErrors["RoomType"] = "Room type is required";
            ok = false;
        }

        setErrors(newErrors);
        return ok;
    };

    async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        validateForm(form);
        setValidated(true);
        if (validateForm(form)) {
            try {
                const response = await createAdminRoom(form);
                alert("Succes! " + response.data);
                navigate("/admin/rooms");
            }
            catch (err: any) {
                alert(err.response.data.message);
            }
        }
    };

    return (
        <div className="register-container">
            <div className="register-box">
                <h2>Create Room (Admin)</h2>
                <form onSubmit={handleSubmit} className="register-form">
                    <input
                        type="number"
                        name="roomNumber"
                        placeholder="Room number"
                        value={form.roomNumber}
                        onChange={handleChange}
                        className={errors["RoomNumber"] ? "invalid" : ""}
                        required
                    />
                    {errors["RoomNumber"] && <span className="error-text">{errors["RoomNumber"]}</span>}

                    <input
                        type="text"
                        name="roomType"
                        placeholder="Room type"
                        value={form.roomType}
                        onChange={handleChange}
                        className={errors["RoomType"] ? "invalid" : ""}
                        required
                    />
                    {errors["RoomType"] && <span className="error-text">{errors["RoomType"]}</span>}

                    {loading ? (
                        <p>Loading...</p>
                    ) : (
                        <Select<SpecializationNameDto, true>
                            isMulti
                            name="specializations"
                            options={specializations}
                            getOptionLabel={(spec) => spec.specializationName}
                            getOptionValue={(spec) => spec.id}
                            value={form.specializations}
                            onChange={(selected) => {
                                setForm({ ...form, specializations: selected ?? [] });
                            }}
                        />
                    )}
                    <button type="submit" className="btn btn-primary w-100 mt-3">Create new Room</button>
                </form>
            </div>
        </div>
    );
}
