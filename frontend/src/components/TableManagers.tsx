import { useQuery } from "@tanstack/react-query";
import axios from "axios";

const fetchTableManagers = async () => {
  const { data } = await axios.get("http://localhost:5267/api/tablemanager/all");
  return data;
};

export default function TableManagers() {
  const { data, isLoading, error } = useQuery({
    queryKey: ["tableManagers"],
    queryFn: fetchTableManagers,
    refetchInterval: 1000, // Auto-refresh every 5 seconds
  });

  if (error) return <p className="text-center text-red-500">Error loading data</p>;
  if (!data || data.length === 0) return <p className="text-center text-gray-500">No tables found.</p>;

  return (
    <div className="p-6">
      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
        {data.map((table: any, index: number) => {
          // Conditionally set background color based on table status
          let boxColor = '';

          switch (table.tableStatus.toLowerCase()) {
            case 'play':
              boxColor = 'bg-red-800'; // Red for "play"
              break;
            case 'standby':
              boxColor = 'bg-blue-800'; // Blue for "standby"
              break;
            case 'off':
              boxColor = 'bg-green-800'; // Green for "off"
              break;
            default:
              boxColor = 'bg-gray-300'; // Default color if status is unknown
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
              <p className="text-3xl text-white font-bold mt-2">
                € {table.price}
              </p>
            </div>
          );
        })}
      </div>
    </div>
  );
}

