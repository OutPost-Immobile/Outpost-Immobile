import { FormControl, InputLabel, MenuItem, Select, type SelectChangeEvent } from "@mui/material";
import { useStatusOptions } from "../Hooks/useStatusOptions.ts";

interface StatusDropdownProps {
    value: number | "";
    onChange: (value: number | "") => void;
    disabled?: boolean;
    size?: "small" | "medium";
    label?: string;
    fullWidth?: boolean;
    sx?: object;
}

export const StatusDropdown = ({
    value,
    onChange,
    disabled = false,
    size = "small",
    label = "Status",
    fullWidth = false,
    sx = {}
}: StatusDropdownProps) => {
    const { options, isLoading } = useStatusOptions();

    const handleChange = (e: SelectChangeEvent<number | "">) => {
        const raw = e.target.value;
        onChange(raw === "" ? "" : Number(raw));
    };

    return (
        <FormControl size={size} fullWidth={fullWidth} sx={{ minWidth: 150, ...sx }}>
            <InputLabel id="status-select-label">{label}</InputLabel>
            <Select
                labelId="status-select-label"
                id="status-select"
                value={value}
                label={label}
                onChange={handleChange}
                disabled={disabled || isLoading}
                displayEmpty={false}
            >
                <MenuItem value="" disabled>
                    {isLoading ? "Ładowanie..." : "Wybierz status"}
                </MenuItem>
                {options.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                        {option.label}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
};
