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

// Clock component that displays a 24-hour clock (en-GB locale)
function Clock() {
  const [time, setTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  return (
    <div className="text-9xl font-semibold">
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
    refetchInterval: 1000, // Auto-refresh every second
  });

  const mutation = useMutation({
    mutationFn: updateTableState,
    onSuccess: () => {
      queryClient.invalidateQueries(["tableManagers"]); // Refresh table data after update
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
    return <p className="text-center text-red-500">Error loading data</p>;
  if (!data || data.length === 0)
    return <p className="text-center text-gray-500">No tables found.</p>;

  // Render the first 4 table boxes, then insert the clock to fill positions 5 and 6.
  const firstRow = data.slice(0, 4);
  const remainingTables = data.slice(4);

  return (
    <div className="p-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
        {firstRow.map((table: any) => {
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
              className={`${boxColor} p-4 rounded-xl shadow-md border border-gray-200 flex flex-col justify-between`}
            >
              <h3 className="text-3xl font-semibold text-white">
                {table.tableName} {table.tableNumber}
              </h3>
              <p className="text-2xl text-white mt-2">
                <span className="font-medium">Status:</span> {table.tableStatus}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Play Time:</span> {table.playTime}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Remaining:</span> {table.remainingTime}
              </p>
              <p className="text-3xl text-white font-bold mt-2">
                € {table.price}
              </p>

              {/* Timed Session Input */}
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
                  className="w-full p-2 mt-2 rounded-lg border border-gray-300 text-black"
                />
              )}

              {/* Invisible Dummy Button for "play" state */}
              {table.tableStatus.toLowerCase() === "play" && (
                <button
                  className="invisible opacity-0 w-full px-4 py-2 rounded-lg font-bold"
                  style={{ height: "2.5rem" }}
                >
                  Invisible Button
                </button>
              )}

              {/* Control Buttons */}
              <div className="mt-4 flex flex-col space-y-2">
                {table.tableStatus.toLowerCase() === "off" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "play")}
                    className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold w-full"
                  >
                    Play
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "play" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "standby")}
                    className="bg-white text-blue-700 px-4 py-2 rounded-lg font-bold w-full"
                  >
                    Standby
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "standby" && (
                  <>
                    <button
                      onClick={() => handleStateChange(table.tableId, "play")}
                      className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold w-full"
                    >
                      Play
                    </button>
                    <button
                      onClick={() => handleStateChange(table.tableId, "off")}
                      className="bg-white text-red-700 px-4 py-2 rounded-lg font-bold w-full"
                    >
                      Off
                    </button>
                  </>
                )}
              </div>
            </div>
          );
        })}

        {/* Clock: Visible only on large screens.
            It spans two grid columns (positions 5 and 6 in the first row) */}
        <div
          key="clock"
          className="hidden lg:flex lg:col-span-2 bg-gray-800 text-white p-4 rounded-xl shadow-md border border-gray-200 items-center justify-center"
        >
          <Clock />
        </div>

        {remainingTables.map((table: any) => {
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
              className={`${boxColor} p-4 rounded-xl shadow-md border border-gray-200 flex flex-col justify-between`}
            >
              <h3 className="text-3xl font-semibold text-white">
                {table.tableName} {table.tableNumber}
              </h3>
              <p className="text-2xl text-white mt-2">
                <span className="font-medium">Status:</span> {table.tableStatus}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Play Time:</span> {table.playTime}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Remaining:</span> {table.remainingTime}
              </p>
              <p className="text-3xl text-white font-bold mt-2">
                € {table.price}
              </p>

              {/* Timed Session Input */}
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
                  className="w-full p-2 mt-2 rounded-lg border border-gray-300 text-black"
                />
              )}

              {/* Invisible Dummy Button for "play" state */}
              {table.tableStatus.toLowerCase() === "play" && (
                <button
                  className="invisible opacity-0 w-full px-4 py-2 rounded-lg font-bold"
                  style={{ height: "2.5rem" }}
                >
                  Invisible Button
                </button>
              )}

              {/* Control Buttons */}
              <div className="mt-4 flex flex-col space-y-2">
                {table.tableStatus.toLowerCase() === "off" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "play")}
                    className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold w-full"
                  >
                    Play
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "play" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "standby")}
                    className="bg-white text-blue-700 px-4 py-2 rounded-lg font-bold w-full"
                  >
                    Standby
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "standby" && (
                  <>
                    <button
                      onClick={() => handleStateChange(table.tableId, "play")}
                      className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold w-full"
                    >
                      Play
                    </button>
                    <button
                      onClick={() => handleStateChange(table.tableId, "off")}
                      className="bg-white text-red-700 px-4 py-2 rounded-lg font-bold w-full"
                    >
                      Off
                    </button>
                  </>
                )}
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

