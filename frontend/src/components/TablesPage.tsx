import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import axios from "axios";

type PoolTable = {
  id: string;
  name: string;
  number: number;
  scheduleId?: string;
};

type ScheduleDTO = {
  id: string;
  name: string;
};

const fetchTables = async () => {
  const { data } = await axios.get<PoolTable[]>(
    "http://localhost:5267/api/tables"
  );
  return data;
};

const fetchSchedules = async () => {
  const { data } = await axios.get<ScheduleDTO[]>(
    "http://localhost:5267/api/schedules"
  );
  return data;
};

const addTable = async (table: { name: string; number: number }) => {
  await axios.post("http://localhost:5267/api/tables", table);
};

const deleteTable = async (id: string) => {
  await axios.delete(`http://localhost:5267/api/tables/${id}`);
};

const assignSchedule = async ({
  tableId,
  scheduleId,
}: {
  tableId: string;
  scheduleId: string;
}) => {
  await axios.put(
    `http://localhost:5267/api/tables/${tableId}/schedule/${scheduleId}`
  );
};

export default function TablesPage() {
  const queryClient = useQueryClient();
  const [name, setName] = useState("Pool");
  const [number, setNumber] = useState<number>(1);
  const [page, setPage] = useState(0);

  const { data: tables, isLoading, error } = useQuery({
    queryKey: ["tables"],
    queryFn: fetchTables,
  });

  const { data: schedules } = useQuery({
    queryKey: ["schedules"],
    queryFn: fetchSchedules,
  });

  const addMutation = useMutation({
    mutationFn: addTable,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
      setNumber((prev) => prev + 1);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteTable,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
    },
  });

  const assignScheduleMutation = useMutation({
    mutationFn: assignSchedule,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
    },
  });

  const handleAdd = () => {
    if (!name.trim()) return;
    addMutation.mutate({ name, number });
  };

  const tablesPerPage = 50;
  const sortedTables = [...(tables ?? [])].sort((a, b) => {
    const nameCompare = a.name.localeCompare(b.name);
    return nameCompare !== 0 ? nameCompare : a.number - b.number;
  });

  const pagedTables = sortedTables.slice(
    page * tablesPerPage,
    (page + 1) * tablesPerPage
  );

  return (
    <div className="p-6 max-w-screen-xl mx-auto">
      <h1 className="text-3xl font-bold mb-4 dark:text-white">Manage Tables</h1>

      {/* Add Table Form */}
      <div className="mb-6 flex items-center gap-4 flex-wrap">
        <input
          type="text"
          value={name}
          placeholder="Table Name"
          onChange={(e) => setName(e.target.value)}
          className="p-2 rounded border border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
        <input
          type="number"
          value={number}
          placeholder="Table Number"
          onChange={(e) => setNumber(parseInt(e.target.value))}
          className="p-2 rounded border border-gray-300 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
        <button
          onClick={handleAdd}
          className="bg-blue-600 text-white px-4 py-2 rounded font-semibold hover:bg-blue-700"
        >
          Add Table
        </button>
      </div>

      {/* Table Cards Grid */}
      {isLoading ? (
        <p className="dark:text-gray-300">Loading tables...</p>
      ) : error ? (
        <p className="text-red-500">Error loading tables.</p>
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-6">
            {pagedTables.map((table) => {
              const activeSchedule = schedules?.find(
                (s) => s.id === table.scheduleId
              );
              return (
                <div
                  key={table.id}
                  className="relative bg-gray-100 p-4 pt-8 rounded-xl shadow-md border border-gray-300 dark:bg-gray-800 dark:border-gray-700 min-h-[180px]"
                >
                  {/* Delete Button */}
                  <button
                    onClick={() => deleteMutation.mutate(table.id)}
                    className="absolute top-2 right-2 text-white bg-red-600 hover:bg-red-700 rounded-full w-6 h-6 flex items-center justify-center font-bold"
                    title="Delete Table"
                  >
                    ×
                  </button>

                  <h2 className="text-xl font-semibold mb-2 dark:text-white">{table.name}</h2>
                  <p className="text-lg mb-1 dark:text-gray-300">Number: {table.number}</p>

                  {schedules && (
                    <>
                      <p className="text-sm mb-1 text-gray-600 dark:text-gray-400">
                        Schedule:{" "}
                        <span
                          className={`font-medium ${
                            activeSchedule ? "text-gray-800 dark:text-gray-200" : "text-red-600"
                          }`}
                        >
                          {activeSchedule?.name ?? "None"}
                        </span>
                      </p>
                      <select
                        defaultValue=""
                        className="w-full p-2 rounded border border-gray-300 text-sm dark:bg-gray-700 dark:text-white dark:border-gray-600"
                        onChange={(e) => {
                          const selectedScheduleId = e.target.value;
                          if (selectedScheduleId) {
                            assignScheduleMutation.mutate({
                              tableId: table.id,
                              scheduleId: selectedScheduleId,
                            });
                          }
                        }}
                      >
                        <option value="" disabled>
                          Assign New Schedule
                        </option>
                        {schedules.map((s) => (
                          <option key={s.id} value={s.id}>
                            {s.name}
                          </option>
                        ))}
                      </select>
                    </>
                  )}
                </div>
              );
            })}
          </div>

          {/* Pagination */}
          <div className="mt-6 flex justify-center items-center gap-4">
            <button
              onClick={() => setPage((p) => Math.max(p - 1, 0))}
              disabled={page === 0}
              className="px-3 py-1 bg-gray-300 rounded disabled:opacity-50 dark:bg-gray-700 dark:text-white"
            >
              Prev
            </button>
            <span className="text-lg font-semibold dark:text-white">Page {page + 1}</span>
            <button
              onClick={() =>
                setPage((p) =>
                  (p + 1) * tablesPerPage >= sortedTables.length ? p : p + 1
                )
              }
              disabled={(page + 1) * tablesPerPage >= sortedTables.length}
              className="px-3 py-1 bg-gray-300 rounded disabled:opacity-50 dark:bg-gray-700 dark:text-white"
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
}

