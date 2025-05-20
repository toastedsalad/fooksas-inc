import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import axios from "axios";

type PoolTable = {
  id: string;
  name: string;
  number: number;
};

const fetchTables = async () => {
  const { data } = await axios.get<PoolTable[]>(
    "http://localhost:5267/api/tables"
  );
  return data;
};

const addTable = async (table: { name: string; number: number }) => {
  await axios.post("http://localhost:5267/api/tables", table);
};

const deleteTable = async (id: string) => {
  await axios.delete(`http://localhost:5267/api/tables/${id}`);
};

export default function TablesPage() {
  const queryClient = useQueryClient();
  const [name, setName] = useState("Pool");
  const [number, setNumber] = useState<number>(1);
  const [page, setPage] = useState(0);

  const { data, isLoading, error } = useQuery({
    queryKey: ["tables"],
    queryFn: fetchTables,
  });

  const addMutation = useMutation({
    mutationFn: addTable,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
      setNumber((prev) => prev + 1); // auto-increment for convenience
    },
  });

  const deleteMutation = useMutation({
    mutationFn: deleteTable,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
    },
  });

  const handleAdd = () => {
    if (!name.trim()) return;
    addMutation.mutate({ name, number });
  };

  const tablesPerPage = 50;
  const rowsPerPage = 5;
  const tablesPerRow = 10;

  const sortedTables = [...(data ?? [])].sort((a, b) => {
    const nameCompare = a.name.localeCompare(b.name);
    return nameCompare !== 0 ? nameCompare : a.number - b.number;
  });

  const pagedTables = sortedTables.slice(
    page * tablesPerPage,
    (page + 1) * tablesPerPage
  );

  return (
    <div className="p-6 max-w-screen-xl mx-auto">
      <h1 className="text-3xl font-bold mb-4">Manage Tables</h1>

      {/* Add Table Form */}
      <div className="mb-6 flex items-center gap-4 flex-wrap">
        <input
          type="text"
          value={name}
          placeholder="Table Name"
          onChange={(e) => setName(e.target.value)}
          className="p-2 rounded border border-gray-300"
        />
        <input
          type="number"
          value={number}
          placeholder="Table Number"
          onChange={(e) => setNumber(parseInt(e.target.value))}
          className="p-2 rounded border border-gray-300"
        />
        <button
          onClick={handleAdd}
          className="bg-blue-600 text-white px-4 py-2 rounded font-semibold"
        >
          Add Table
        </button>
      </div>

      {/* Table Cards Grid */}
      {isLoading ? (
        <p>Loading tables...</p>
      ) : error ? (
        <p className="text-red-500">Error loading tables.</p>
      ) : (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-5 lg:grid-cols-10 gap-4">
            {pagedTables.map((table) => (
              <div
                key={table.id}
                className="relative bg-gray-100 p-4 pt-8 rounded-xl shadow-md border border-gray-300"
              >
                {/* Delete Button */}
                <button
                  onClick={() => deleteMutation.mutate(table.id)}
                  className="absolute top-2 right-2 text-white bg-red-600 hover:bg-red-700 rounded-full w-6 h-6 flex items-center justify-center font-bold"
                  title="Delete Table"
                >
                  ×
                </button>

                <h2 className="text-xl font-semibold mb-2">{table.name}</h2>
                <p className="text-lg">Number: {table.number}</p>
              </div>
            ))}
          </div>

          {/* Pagination */}
          <div className="mt-6 flex justify-center items-center gap-4">
            <button
              onClick={() => setPage((p) => Math.max(p - 1, 0))}
              disabled={page === 0}
              className="px-3 py-1 bg-gray-300 rounded disabled:opacity-50"
            >
              Prev
            </button>
            <span className="text-lg font-semibold">Page {page + 1}</span>
            <button
              onClick={() =>
                setPage((p) =>
                  (p + 1) * tablesPerPage >= sortedTables.length ? p : p + 1
                )
              }
              disabled={(page + 1) * tablesPerPage >= sortedTables.length}
              className="px-3 py-1 bg-gray-300 rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
        </>
      )}
    </div>
  );
}

