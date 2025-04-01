import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import axios from "axios";

const fetchTableManagers = async () => {
  const { data } = await axios.get("http://localhost:5267/api/tablemanager/all");
  return data;
};

const updateTableState = async ({ tableId, newState }: { tableId: number; newState: string }) => {
  await axios.put(`http://localhost:5267/api/tablemanager/${tableId}/${newState}`);
};

export default function TableManagers() {
  const queryClient = useQueryClient();

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
    mutation.mutate({ tableId, newState });
  };

  if (error) return <p className="text-center text-red-500">Error loading data</p>;
  if (!data || data.length === 0) return <p className="text-center text-gray-500">No tables found.</p>;

  return (
    <div className="p-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-6">
        {data.map((table: any, index: number) => {
          // Conditionally set background color based on table status
          let boxColor = "";

          switch (table.tableStatus.toLowerCase()) {
            case "play":
              boxColor = "bg-red-800"; // Darker Red
              break;
            case "standby":
              boxColor = "bg-blue-800"; // Darker Blue
              break;
            case "off":
              boxColor = "bg-green-800"; // Darker Green
              break;
            default:
              boxColor = "bg-gray-500"; // Default Gray
              break;
          }

          return (
            <div
              key={index}
              className={`${boxColor} p-4 rounded-xl shadow-md border border-gray-200`}
            >
              <h3 className="text-3xl font-semibold text-white">Table {table.tableId}</h3>
              <p className="text-2xl text-white mt-2">
                <span className="font-medium">Status:</span> {table.tableStatus}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Play Time:</span> {table.playTime}
              </p>
              <p className="text-2xl text-white">
                <span className="font-medium">Remaining:</span> {table.remainingTime}
              </p>
              <p className="text-3xl text-white font-bold mt-2">€ {table.price}</p>

              {/* Control Buttons */}
              <div className="mt-4 flex space-x-2">
                {table.tableStatus.toLowerCase() === "off" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "play")}
                    className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold"
                  >
                    Play
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "play" && (
                  <button
                    onClick={() => handleStateChange(table.tableId, "standby")}
                    className="bg-white text-blue-700 px-4 py-2 rounded-lg font-bold"
                  >
                    Standby
                  </button>
                )}
                {table.tableStatus.toLowerCase() === "standby" && (
                  <>
                    <button
                      onClick={() => handleStateChange(table.tableId, "play")}
                      className="bg-white text-green-700 px-4 py-2 rounded-lg font-bold"
                    >
                      Play
                    </button>
                    <button
                      onClick={() => handleStateChange(table.tableId, "off")}
                      className="bg-white text-red-700 px-4 py-2 rounded-lg font-bold"
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
