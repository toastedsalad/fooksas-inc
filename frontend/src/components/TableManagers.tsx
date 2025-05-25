import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import axios from "axios";

const fetchTableManagers = async () => {
  const { data } = await axios.get("http://localhost:5267/api/tablemanager/all");
  return data;
};

const updateTableState = async ({
  tableId,
  newState,
  timedSeconds = 0,
}: {
  tableId: number;
  newState: string;
  timedSeconds?: number;
}) => {
  await axios.put(`http://localhost:5267/api/tablemanager/${tableId}/${newState}`, null, {
    params: { timedSeconds },
  });
};

function Clock() {
  const [time, setTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  return (
    <div className="text-9xl font-semibold text-gray-200">
      {time.toLocaleTimeString("en-GB")}
    </div>
  );
}

export default function TableManagers() {
  const queryClient = useQueryClient();
  const [timedSessions, setTimedSessions] = useState<{ [key: number]: string }>({});

  const { data, isLoading, error } = useQuery({
    queryKey: ["tableManagers"],
    queryFn: fetchTableManagers,
    refetchInterval: 1000,
  });

  const mutation = useMutation({
    mutationFn: updateTableState,
    onSuccess: () => {
      queryClient.invalidateQueries(["tableManagers"]);
    },
  });

  const handleStateChange = (tableId: number, newState: string) => {
    if (newState === "play") {
      const timedSeconds = parseInt(timedSessions[tableId] || "0") * 60;
      mutation.mutate({ tableId, newState, timedSeconds });
      setTimedSessions((prev) => ({ ...prev, [tableId]: "" }));
    } else {
      mutation.mutate({ tableId, newState });
    }
  };

  if (error)
    return <p className="text-center text-red-500 dark:text-red-400">Error loading data</p>;
  if (!data || data.length === 0)
    return <p className="text-center text-gray-500 dark:text-gray-400">No tables found.</p>;

  const firstRow = data.slice(0, 5);
  const remainingTables = data.slice(5);

  const renderTableCard = (table: any) => {
    let boxColor = "";
    switch (table.tableStatus.toLowerCase()) {
      case "play":
        boxColor = "bg-red-800";
        break;
      case "standby":
        boxColor = "bg-blue-800";
        break;
      case "off":
        boxColor = "bg-green-800";
        break;
      default:
        boxColor = "bg-gray-500";
        break;
    }

    return (
      <div
        key={table.tableId}
        className={`${boxColor} relative p-4 rounded-xl shadow-md border border-gray-200 dark:border-gray-700 flex flex-col justify-between`}
      >
        <div className="absolute top-2 right-4 text-white text-5xl font-extrabold">
          {table.tableNumber}
        </div>

        <h3 className="text-3xl font-semibold text-white">{table.tableName}</h3>
        <p className="text-2xl text-white mt-2">
          <span className="font-medium">Status:</span> {table.tableStatus}
        </p>
        <p className="text-2xl text-white">
          <span className="font-medium">Play Time:</span> {table.playTime}
        </p>
        <p className="text-2xl text-white">
          <span className="font-medium">Remaining:</span> {table.remainingTime}
        </p>
        <p className="text-4xl text-white font-extrabold mt-4">€ {table.price}</p>

        {table.tableStatus.toLowerCase() === "off" && (
          <input
            type="number"
            value={timedSessions[table.tableId] || ""}
            onChange={(e) =>
              setTimedSessions((prev) => ({
                ...prev,
                [table.tableId]: e.target.value,
              }))
            }
            placeholder="Minutes (optional)"
            className="w-full p-2 mt-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-black dark:text-white"
          />
        )}

        {table.tableStatus.toLowerCase() === "play" && (
          <button
            className="invisible opacity-0 w-full px-4 py-2 rounded-lg font-bold"
            style={{ height: "2.5rem" }}
          >
            Invisible Button
          </button>
        )}

        <div className="mt-4 flex flex-col space-y-2">
          {table.tableStatus.toLowerCase() === "off" && (
            <button
              onClick={() => handleStateChange(table.tableId, "play")}
              className="bg-white dark:bg-gray-800 dark:text-green-400 text-green-700 px-4 py-2 rounded-lg font-bold w-full"
            >
              Play
            </button>
          )}
          {table.tableStatus.toLowerCase() === "play" && (
            <button
              onClick={() => handleStateChange(table.tableId, "standby")}
              className="bg-white dark:bg-gray-800 dark:text-blue-400 text-blue-700 px-4 py-2 rounded-lg font-bold w-full"
            >
              Standby
            </button>
          )}
          {table.tableStatus.toLowerCase() === "standby" && (
            <>
              <button
                onClick={() => handleStateChange(table.tableId, "play")}
                className="bg-white dark:bg-gray-800 dark:text-green-400 text-green-700 px-4 py-2 rounded-lg font-bold w-full"
              >
                Play
              </button>
              <button
                onClick={() => handleStateChange(table.tableId, "off")}
                className="bg-white dark:bg-gray-800 dark:text-red-400 text-red-700 px-4 py-2 rounded-lg font-bold w-full"
              >
                Off
              </button>
            </>
          )}
        </div>
      </div>
    );
  };

  return (
    <div className="p-6 bg-white dark:bg-gray-900 min-h-screen">
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 xl:grid-cols-8 gap-6">
        {/* First Row: 5 table cards + clock */}
        {firstRow.map(renderTableCard)}

        <div
          key="clock"
          className="hidden xl:flex col-span-3 bg-gray-800 dark:bg-gray-700 text-white dark:text-gray-200 p-4 rounded-xl shadow-md border border-gray-200 dark:border-gray-700 items-center justify-center"
        >
          <Clock />
        </div>

        {/* Remaining cards in rows of 8 */}
        {remainingTables.map(renderTableCard)}
      </div>
    </div>
  );
}

