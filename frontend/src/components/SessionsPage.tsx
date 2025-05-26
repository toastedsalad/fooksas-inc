import axios from "axios";
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

const apiBase = "http://localhost:5267/api/sessions";

type PlaySession = {
  id: string;
  startTime: string;
  playTime: string;
  price: number;
  tableNumber: number;
  playerId: string;
};

function useDarkMode() {
  const [isDark, setIsDark] = useState(false);
  useEffect(() => {
    const root = document.documentElement;
    const observer = new MutationObserver(() => {
      setIsDark(root.classList.contains("dark"));
    });
    observer.observe(root, { attributes: true, attributeFilter: ["class"] });
    setIsDark(root.classList.contains("dark"));
    return () => observer.disconnect();
  }, []);
  return isDark;
}

function toISOStringLocal(date: Date): string {
  const offset = date.getTimezoneOffset();
  const local = new Date(date.getTime() - offset * 60 * 1000);
  return local.toISOString().slice(0, 16);
}

export default function SessionsPage() {
  const [startDate, setStartDate] = useState<Date | null>(null);
  const [endDate, setEndDate] = useState<Date | null>(null);
  const isDarkMode = useDarkMode();
  const [preset, setPreset] = useState<"24h" | "7d" | "30d" | null>(null);

  useEffect(() => {
    const now = new Date();
    let from: Date;

    if (preset === "24h") {
      from = new Date(now.getTime() - 24 * 60 * 60 * 1000);
    } else if (preset === "7d") {
      from = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
    } else if (preset === "30d") {
      from = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000);
    } else {
      return;
    }

    setStartDate(from);
    setEndDate(now);
  }, [preset]);

  const { data: sessions, isLoading } = useQuery({
    queryKey: ["sessions", startDate, endDate],
    queryFn: async () => {
      if (!startDate || !endDate) return [];
      const { data } = await axios.get<PlaySession[]>(`${apiBase}/range`, {
        params: {
          start: startDate.toISOString(),
          end: endDate.toISOString(),
        },
      });
      return data;
    },
    enabled: !!startDate && !!endDate,
  });

  const downloadCsv = async () => {
    if (!startDate || !endDate) return;
    const start = encodeURIComponent(startDate.toISOString());
    const end = encodeURIComponent(endDate.toISOString());
    const url = `${apiBase}/range/csv?start=${start}&end=${end}`;

    try {
      const response = await axios.get(url, { responseType: "blob" });
      const blob = new Blob([response.data], { type: "text/csv" });
      const link = document.createElement("a");
      link.href = window.URL.createObjectURL(blob);
      link.download = `sessions_${start}_${end}.csv`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (err) {
      console.error("CSV download failed:", err);
    }
  };

  return (
    <div className="p-6 max-w-6xl mx-auto text-gray-900 dark:text-gray-100 space-y-6">
      <h1 className="text-3xl font-bold">Session Browser</h1>

      {/* Date Pickers + Presets */}
      <div className="flex flex-wrap items-center gap-4">
        <label className="flex flex-col">
          Start Time
          <DatePicker
            selected={startDate}
            onChange={(date) => setStartDate(date)}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            dateFormat="yyyy-MM-dd HH:mm"
            timeCaption="Time"
            className="bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 border rounded p-2 border-gray-300 dark:border-gray-600"
          />
        </label>

        <label className="flex flex-col">
          End Time
          <DatePicker
            selected={endDate}
            onChange={(date) => setEndDate(date)}
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={15}
            dateFormat="yyyy-MM-dd HH:mm"
            timeCaption="Time"
            className="bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 border rounded p-2 border-gray-300 dark:border-gray-600"
          />
        </label>

        <div className="flex gap-2 mt-4 lg:mt-6">
          {(["24h", "7d", "30d"] as const).map((p) => (
            <button
              key={p}
              onClick={() => setPreset(p)}
              className={`px-3 py-1 text-sm rounded border transition ${
                preset === p
                  ? "bg-blue-600 text-white border-blue-600"
                  : "bg-gray-100 dark:bg-gray-700 border-gray-300 dark:border-gray-600 text-gray-800 dark:text-gray-100"
              }`}
            >
              Last {p}
            </button>
          ))}
        </div>
      </div>

      {/* Download CSV Button */}
      <button
        onClick={downloadCsv}
        disabled={!startDate || !endDate}
        className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
      >
        Download CSV
      </button>

      {/* Session List */}
      <div className="border rounded p-4 bg-gray-50 dark:bg-gray-800 shadow max-h-[600px] overflow-y-auto space-y-2 max-w-2xl w-full">
        {isLoading ? (
          <p>Loading...</p>
        ) : sessions?.length === 0 ? (
          <p>No sessions found in selected range.</p>
        ) : (
          sessions?.slice(0, 100000).map((s) => (
            <div
              key={s.id}
              className="p-3 rounded border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 flex flex-col md:flex-row justify-between items-start md:items-center"
            >
              <div className="space-y-1">
                <p className="font-medium">
                  Table #{s.tableNumber} | Player: {s.playerId.slice(0, 6)}...
                </p>
                <p className="text-sm">
                  Start:{" "}
                  {new Date(s.startTime)
                    .toLocaleString("lt-LT", {
                      year: "numeric",
                      month: "2-digit",
                      day: "2-digit",
                      hour: "2-digit",
                      minute: "2-digit",
                      hour12: false,
                    })
                    .replace(",", "")}
                </p>
                <p className="text-sm">Duration: {s.playTime}</p>
              </div>
              <div className="text-lg font-bold text-green-600 dark:text-green-400">
                ${s.price.toFixed(2)}
              </div>
            </div>
          ))
        )}
        {sessions && sessions.length > 100000 && (
          <div className="text-center text-sm text-gray-500 mt-2">
            Showing first 100000 sessions.
          </div>
        )}
      </div>
    </div>
  );
}

