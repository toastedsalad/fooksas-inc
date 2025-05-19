import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";

type PoolTable = {
  id: string;
  name: string;
  number: number;
};

const fetchTables = async () => {
  const { data } = await axios.get("http://localhost:5267/api/tables");
  return data;
};

const addTable = async (newTable: { number: number; name: string }) => {
  await axios.post("http://localhost:5267/api/tables", newTable);
};

export default function TablesPage() {
  const queryClient = useQueryClient();

  const { data: tables = [], isLoading } = useQuery({
    queryKey: ["tables"],
    queryFn: fetchTables,
  });

  const [tableNumber, setTableNumber] = useState("");
  const [tableName, setTableName] = useState("Pool");
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 50; // <-- Updated to 50

  const mutation = useMutation({
    mutationFn: addTable,
    onSuccess: () => {
      queryClient.invalidateQueries(["tables"]);
      setTableNumber("");
      setTableName("Pool");
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const number = parseInt(tableNumber);
    if (!isNaN(number)) {
      mutation.mutate({ number, name: tableName });
    }
  };

  const paginatedTables = tables.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  const totalPages = Math.ceil(tables.length / itemsPerPage);

  return (
    <div className="p-6 space-y-8">
      <h2 className="text-3xl font-bold">Add a New Table</h2>

      <form
        onSubmit={handleSubmit}
        className="space-y-4 max-w-md bg-white p-4 rounded shadow"
      >
        <div>
          <label className="block font-semibold">Table Number</label>
          <input
            type="text"
            value={tableNumber}
            onChange={(e) => {
              const val = e.target.value;
              if (/^\d*$/.test(val)) setTableNumber(val);
            }}
            placeholder="e.g. 5"
            className="w-full p-2 border rounded appearance-none"
          />
        </div>

        <div>
          <label className="block font-semibold">Table Name</label>
          <input
            type="text"
            value={tableName}
            onChange={(e) => setTableName(e.target.value)}
            className="w-full p-2 border rounded"
          />
        </div>

        <button
          type="submit"
          className="bg-green-600 text-white px-4 py-2 rounded font-bold hover:bg-green-700"
        >
          Add Table
        </button>
      </form>

      <h2 className="text-2xl font-semibold">Existing Tables</h2>

      {isLoading ? (
        <p>Loading...</p>
      ) : (
        <div>
          <ul className="grid grid-cols-2 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-10 gap-4">
            {paginatedTables.map((table: PoolTable) => (
              <li
                key={table.id}
                className="border p-4 rounded shadow bg-gray-50 text-center"
              >
                <p className="text-xl font-bold">#{table.number}</p>
                <p className="text-gray-700">{table.name}</p>
              </li>
            ))}
          </ul>

          <div className="flex justify-center items-center gap-2 mt-6">
            <button
              disabled={currentPage === 1}
              onClick={() => setCurrentPage((p) => p - 1)}
              className="px-3 py-1 rounded bg-blue-500 text-white disabled:opacity-50"
            >
              Prev
            </button>
            <span>
              Page {currentPage} of {totalPages}
            </span>
            <button
              disabled={currentPage === totalPages}
              onClick={() => setCurrentPage((p) => p + 1)}
              className="px-3 py-1 rounded bg-blue-500 text-white disabled:opacity-50"
            >
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

