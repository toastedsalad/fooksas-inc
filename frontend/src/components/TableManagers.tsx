import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useState, useEffect } from "react";
import axios from "axios";

// Types
type Discount = {
  id: string;
  type: string;
  name: string;
  rate: number;
};

const fetchTableManagers = async () => {
  const { data } = await axios.get("http://localhost:5267/api/tablemanager/all");
  return data;
};

const fetchDiscounts = async () => {
  const { data } = await axios.get("http://localhost:5267/api/discount/type/other");
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

const updateDiscount = async ({
  tableId,
  discount,
}: {
  tableId: number;
  discount: Discount;
}) => {
  await axios.put(`http://localhost:5267/api/tablemanager/${tableId}/session/discount/${discount.id}/update`);
};

function Clock() {
  const [time, setTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  return (
    <div className="text-9xl font-semibold text-gray-200 dark:text-gray-100">
      {time.toLocaleTimeString("en-GB")}
    </div>
  );
}

export default function TableManagers() {
  const queryClient = useQueryClient();
  const [timedSessions, setTimedSessions] = useState<{ [key: number]: string }>({});
  const [pendingDiscounts, setPendingDiscounts] = useState<{ [key: number]: Discount | null }>({});
  const [selectedTableId, setSelectedTableId] = useState<number | null>(null);
  const [showDiscountModal, setShowDiscountModal] = useState(false);

  const { data, isLoading, error } = useQuery({
    queryKey: ["tableManagers"],
    queryFn: fetchTableManagers,
    refetchInterval: 1000,
  });

  const { data: discounts } = useQuery({
    queryKey: ["discounts", "other"],
    queryFn: fetchDiscounts,
    enabled: showDiscountModal,
  });

  const stateMutation = useMutation({
    mutationFn: updateTableState,
    onSuccess: () => queryClient.invalidateQueries(["tableManagers"]),
  });

  const discountMutation = useMutation({
    mutationFn: updateDiscount,
    onSuccess: () => queryClient.invalidateQueries(["tableManagers"]),
  });

  const handleStateChange = async (tableId: number, newState: string) => {
    if (newState === "play") {
      const timedSeconds = parseInt(timedSessions[tableId] || "0") * 60;

      await stateMutation.mutateAsync({ tableId, newState, timedSeconds });

      const discount = pendingDiscounts[tableId];
      if (discount) {
        await discountMutation.mutateAsync({ tableId, discount });
        setPendingDiscounts((prev) => {
          const copy = { ...prev };
          delete copy[tableId];
          return copy;
        });
      }

      setTimedSessions((prev) => ({ ...prev, [tableId]: "" }));
    } else {
      stateMutation.mutate({ tableId, newState });
    }
  };

  const openDiscountModal = (tableId: number) => {
    setSelectedTableId(tableId);
    setShowDiscountModal(true);
  };

  const applyDiscount = (discount: Discount) => {
    if (selectedTableId !== null) {
      setPendingDiscounts((prev) => ({
        ...prev,
        [selectedTableId]: discount,
      }));
      setShowDiscountModal(false);

      const table = data.find((t: any) => t.tableId === selectedTableId);
      if (table && table.tableStatus.toLowerCase() !== "off") {
        discountMutation.mutate({ tableId: selectedTableId, discount });
        setPendingDiscounts((prev) => {
          const copy = { ...prev };
          delete copy[selectedTableId];
          return copy;
        });
      }
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

    const discount = pendingDiscounts[table.tableId];

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

        {discount && (
          <p className="text-white text-lg mt-2">
            <span className="font-medium">Selected Discount:</span> {discount.name} ({discount.rate}%)
          </p>
        )}

        <div className="flex items-center justify-between mt-4">
          <p className="text-4xl text-white font-extrabold">€ {table.price}</p>
          <button
            onClick={() => openDiscountModal(table.tableId)}
            className="bg-white dark:bg-gray-800 dark:text-blue-400 text-green-700 px-3 py-2 rounded-lg font-bold text-l hover:bg-green-600 transition"
          >
            %
          </button>
        </div>

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
        {firstRow.map(renderTableCard)}
        <div
          key="clock"
          className="hidden xl:flex col-span-3 bg-gray-800 dark:bg-gray-700 text-white dark:text-gray-200 p-4 rounded-xl shadow-md border border-gray-200 dark:border-gray-700 items-center justify-center"
        >
          <Clock />
        </div>
        {remainingTables.map(renderTableCard)}
      </div>

      {/* Discount Modal */}
      {showDiscountModal && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50">
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg w-96 shadow-lg space-y-4">
            <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">Select Discount</h2>
            {discounts?.map((discount: Discount) => (
              <button
                key={discount.id}
                onClick={() => applyDiscount(discount)}
                className="w-full text-left px-4 py-2 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-900 dark:text-white border border-gray-300 dark:border-gray-600"
              >
                {discount.name} ({discount.rate}%)
              </button>
            ))}
            <button
              onClick={() => setShowDiscountModal(false)}
              className="mt-4 w-full bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded"
            >
              Cancel
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
