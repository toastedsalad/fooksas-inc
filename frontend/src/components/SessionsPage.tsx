import axios from "axios";
import { useQuery } from "@tanstack/react-query";
import { useState, useEffect } from "react";

const apiBase = "http://localhost:5267/api/sessions/range";

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

function toDatetimeLocalString(date: Date): string {
  const offset = date.getTimezoneOffset();
  const local = new Date(date.getTime() - offset * 60 * 1000);
  return local.toISOString().slice(0, 16);
}

export default function SessionsPage() {
  const [start, setStart] = useState<string>("");
  const [end, setEnd] = useState<string>("");
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

    setStart(toDatetimeLocalString(from));
    setEnd(toDatetimeLocalString(now));
  }, [preset]);

  const { data: sessions, isLoading } = useQuery({
    queryKey: ["sessions", start, end],
    queryFn: async () => {
      if (!start || !end) return [];
      const { data } = await axios.get<PlaySession[]>(apiBase, {
        params: { start, end },
      });
      return data;
    },
    enabled: !!start && !!end,
  });

  return (
    <div className="p-6 max-w-6xl mx-auto text-gray-900 dark:text-gray-100 space-y-6">
      <h1 className="text-3xl font-bold">Session Browser</h1>

      {/* Range Inputs + Presets */}
      <div className="flex flex-wrap items-center gap-4">
        <label className="flex flex-col">
          Start Time
          <input
            type="datetime-local"
            className="bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 border rounded p-2 border-gray-300 dark:border-gray-600"
            value={start}
            onChange={(e) => setStart(e.target.value)}
          />
        </label>

        <label className="flex flex-col">
          End Time
          <input
            type="datetime-local"
            className="bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 border rounded p-2 border-gray-300 dark:border-gray-600"
            value={end}
            onChange={(e) => setEnd(e.target.value)}
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

      {/* Sessions List */}
      <div className="border rounded p-4 bg-gray-50 dark:bg-gray-800 shadow max-h-[600px] overflow-y-auto space-y-2">
        {isLoading ? (
          <p>Loading...</p>
        ) : sessions?.length === 0 ? (
          <p>No sessions found in selected range.</p>
        ) : (
          sessions?.slice(0, 50).map((s) => (
            <div
              key={s.id}
              className="p-3 rounded border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-900 flex flex-col md:flex-row justify-between items-start md:items-center"
            >
              <div className="space-y-1">
                <p className="font-medium">
                  Table #{s.tableNumber} | Player: {s.playerId.slice(0, 6)}...
                </p>
                <p className="text-sm">
                  Start: {new Date(s.startTime).toLocaleString()}
                </p>
                <p className="text-sm">
                  Duration: {s.playTime}
                </p>
              </div>
              <div className="text-lg font-bold text-green-600 dark:text-green-400">
                ${s.price.toFixed(2)}
              </div>
            </div>
          ))
        )}
        {sessions && sessions.length > 50 && (
          <div className="text-center text-sm text-gray-500 mt-2">
            Showing first 50 sessions.
          </div>
        )}
      </div>
    </div>
  );
}

