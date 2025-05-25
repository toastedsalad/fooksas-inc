import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useState } from "react";
import Select from "react-select";

const apiBase = "http://localhost:5267/api/schedules";

const daysOfWeek: DayOfWeek[] = [
  "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" 
];

type TimeRate = { start: string; end: string; price: number };
type ScheduleDTO = {
  id: string;
  name: string;
  defaultRate: number;
  weeklyRates: string | null;
};

const timeOptions = Array.from({ length: 24 * 4 }, (_, i) => {
  const hours = String(Math.floor(i / 4)).padStart(2, "0");
  const minutes = String((i % 4) * 15).padStart(2, "0");
  const label = `${hours}:${minutes}`;
  return { value: label, label };
});

export default function SchedulesPage() {
  const queryClient = useQueryClient();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [editable, setEditable] = useState<ScheduleDTO | null>(null);

  const { data: schedules } = useQuery({
    queryKey: ["schedules"],
    queryFn: async () => {
      const { data } = await axios.get<ScheduleDTO[]>(apiBase);
      return data;
    },
  });

  const createMutation = useMutation({
    mutationFn: async () => {
      const newSchedule = {
        name: "New Schedule",
        defaultRate: 5.0,
        weeklyRates: "{}"
      };
      const { data } = await axios.post(apiBase, newSchedule);
      return data;
    },
    onSuccess: () => queryClient.invalidateQueries(["schedules"]),
  });

  const updateMutation = useMutation({
    mutationFn: async (schedule: ScheduleDTO) => {
      await axios.post(apiBase, schedule);
    },
    onSuccess: () => queryClient.invalidateQueries(["schedules"]),
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await axios.delete(`${apiBase}/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["schedules"]);
      if (selectedId === editable?.id) {
        setEditable(null);
        setSelectedId(null);
      }
    },
  });

  const handleSelect = (id: string) => {
    const sched = schedules?.find((s) => s.id === id);
    if (sched) {
      setEditable({ ...sched });
      setSelectedId(id);
    }
  };

  const getRatesObj = (): Record<string, TimeRate[]> =>
    editable?.weeklyRates ? JSON.parse(editable.weeklyRates) : {};

  const updateRates = (day: string, idx: number, key: keyof TimeRate, value: string | number) => {
    if (!editable) return;
    const rates = getRatesObj();
    if (!rates[day]) rates[day] = [];
    const copy = [...rates[day]];
    copy[idx] = { ...copy[idx], [key]: value };
    rates[day] = copy;
    setEditable({ ...editable, weeklyRates: JSON.stringify(rates) });
  };

  const addRate = (day: string) => {
    const rates = getRatesObj();
    const dayRates = rates[day] ?? [];
    dayRates.push({ start: "00:00", end: "01:00", price: 5.0 });
    rates[day] = dayRates;
    setEditable({ ...editable!, weeklyRates: JSON.stringify(rates) });
  };

  const removeRate = (day: string, idx: number) => {
    const rates = getRatesObj();
    rates[day]?.splice(idx, 1);
    setEditable({ ...editable!, weeklyRates: JSON.stringify(rates) });
  };

  const isDirty = (() => {
    if (!editable || !selectedId || !schedules) return false;
    const original = schedules.find((s) => s.id === selectedId);
    return JSON.stringify(editable) !== JSON.stringify(original);
  })();

  return (
    <div className="p-6 max-w-7xl mx-auto space-y-6">
      <h1 className="text-3xl font-bold">Manage Schedules</h1>

      <button
        onClick={() => createMutation.mutate()}
        className="bg-green-600 text-white px-4 py-2 rounded"
      >
        + New Schedule
      </button>

      <div className="flex flex-wrap gap-2 mt-4">
        {schedules?.map((sched) => (
          <button
            key={sched.id}
            onClick={() => handleSelect(sched.id)}
            className={`px-3 py-1 rounded border ${
              selectedId === sched.id
                ? "bg-blue-500 text-white"
                : "bg-gray-100"
            }`}
          >
            {sched.name}
          </button>
        ))}
      </div>

      {editable && (
        <div className="flex gap-8 mt-6">
          {/* Editable Schedule Form */}
          <div className="border rounded p-4 bg-gray-50 shadow space-y-4 relative max-w-3xl">
            {isDirty && (
              <div className="absolute top-2 right-4 text-orange-600 font-medium">
                Unsaved changes
              </div>
            )}

            <div className="flex gap-4">
              <label className="flex items-center gap-2 w-80">
                <span className="whitespace-nowrap font-medium">Name:</span>
                <input
                  value={editable.name}
                  onChange={(e) =>
                    setEditable({ ...editable, name: e.target.value })
                  }
                  className="p-2 border rounded w-full"
                  placeholder="Schedule Name"
                />
              </label>

              <label className="flex items-center gap-2 w-60">
                <span className="whitespace-nowrap font-medium">Default Rate:</span>
                <input
                  type="number"
                  value={editable.defaultRate}
                  onChange={(e) =>
                    setEditable({ ...editable, defaultRate: parseFloat(e.target.value) })
                  }
                  className="p-2 border rounded w-full"
                  placeholder="Default Rate"
                />
              </label>
            </div>

            {daysOfWeek.map((day) => {
              const rates = getRatesObj()[day] ?? [];
              return (
                <div key={day}>
                  <h3 className="text-lg font-semibold mt-4">{day}</h3>
                  {rates.map((rate, idx) => (
                    <div key={idx} className="flex gap-2 items-center my-1">
                      <Select
                        className="w-32"
                        options={timeOptions}
                        value={timeOptions.find((opt) => opt.value === rate.start)}
                        onChange={(selected) => updateRates(day, idx, "start", selected?.value || "00:00")}
                      />
                      <Select
                        className="w-32"
                        options={timeOptions}
                        value={timeOptions.find((opt) => opt.value === rate.end)}
                        onChange={(selected) => updateRates(day, idx, "end", selected?.value || "00:00")}
                      />
                      <input
                        type="number"
                        value={rate.price}
                        onChange={(e) =>
                          updateRates(day, idx, "price", parseFloat(e.target.value))
                        }
                        className="p-1 border rounded w-24"
                      />
                      <button
                        onClick={() => removeRate(day, idx)}
                        className="text-red-600 font-bold"
                      >
                        ×
                      </button>
                    </div>
                  ))}
                  <button
                    onClick={() => addRate(day)}
                    className="text-blue-600 text-sm mt-1"
                  >
                    + Add Rate
                  </button>
                </div>
              );
            })}

            <div className="flex gap-4 mt-4">
              {isDirty && (
                <button
                  onClick={() => updateMutation.mutate(editable)}
                  className="bg-blue-600 text-white px-4 py-2 rounded"
                >
                  Save Changes
                </button>
              )}
              <button
                onClick={() => deleteMutation.mutate(editable.id)}
                className="bg-red-600 text-white px-4 py-2 rounded"
              >
                Delete Schedule
              </button>
            </div>
          </div>

          {/* Instructional Sidebar */}
          <div className="w-80 p-4 bg-white border rounded shadow-sm text-sm leading-relaxed">
            <h2 className="text-lg font-semibold mb-2">Kaip veikia laikotarpiai ir tarifai?</h2>
            <p>
              Kiekviena diena gali turėti kelis laikotarpius su specifine kaina.
              Kiekvienas laikotarpis turi pradžią <strong>"start time"</strong>, pabaigą <strong>"end time"</strong>, ir kainą <strong>"rate"</strong>.
            </p>
            <ul className="list-disc ml-5 mt-2 space-y-1">
              <li>Laiko formatas yra 24 valandų <code>VV:mm</code>, pvz <code>08:00</code></li>
              <li>Kai žaidimo laikui nėra priskirtas specifinis laikotarpis, jam bus taikomas įprastinis tarifas: <strong>"Default Rate"</strong>. Įprastinis tarifas veikia visada išskyrus kai yra nustatytas specifinis laikotarpis su kaina.</li>
              <li>Laikotarpio pagaigos laikas yra neimtinas, t.y. laikotarpio <strong>"end time"</strong> veikia <strong>iki</strong> nustatyto laiko. Jeigu laikotarpio pagaiga yra nustatyta kaip 15:00, sistema šitą <strong>"end time"</strong> pakeis į 14:59:59.</li>
            </ul>
          </div>
        </div>
      )}
    </div>
  );
}

