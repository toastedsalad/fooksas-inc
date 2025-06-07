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

type Table = {
  tableId: number;
  tableName: string;
  tableNumber: number;
  tableStatus: string;
  playTime: string;
  remainingTime: string;
  price: number;
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

const ActionButton = ({
  onClick,
  children,
  colorClasses,
}: {
  onClick: () => void;
  children: React.ReactNode;
  colorClasses: string;
}) => (
  <button
    onClick={onClick}
    className={`bg-white dark:bg-gray-800 ${colorClasses} px-4 py-2 rounded-lg font-bold w-full`}
  >
    {children}
  </button>
);

export default function TableManagers() {
  const queryClient = useQueryClient();
  const [timedSessions, setTimedSessions] = useState<{ [key: number]: string }>({});
  const [pendingDiscounts, setPendingDiscounts] = useState<{ [key: number]: Discount | null }>({});
  const [selectedTableId, setSelectedTableId] = useState<number | null>(null);
  const [showDiscountModal, setShowDiscountModal] = useState(false);
  const [discountModalTab, setDiscountModalTab] = useState<"discounts" | "players">("discounts");
  const [playerFilters, setPlayerFilters] = useState({ name: "", surname: "", email: "" });
  const [selectedPlayerId, setSelectedPlayerId] = useState<string | null>(null);

  const { data, isLoading, error } = useQuery({
    queryKey: ["tableManagers"],
    queryFn: fetchTableManagers,
    refetchInterval: 3000,
  });

  const { data: discounts, isLoading: isLoadingDiscounts } = useQuery({
    queryKey: ["discounts", "other"],
    queryFn: fetchDiscounts,
    enabled: showDiscountModal && discountModalTab === "discounts",
  });

  const { data: players, isLoading: isSearchingPlayers } = useQuery({
    queryKey: ["searchPlayers", playerFilters],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (playerFilters.name) params.append("name", playerFilters.name);
      if (playerFilters.surname) params.append("surname", playerFilters.surname);
      if (playerFilters.email) params.append("email", playerFilters.email);
      const res = await axios.get(`http://localhost:5267/api/player/search?${params}`);
      return res.data;
    },
    enabled: showDiscountModal && discountModalTab === "players",
  });

  const stateMutation = useMutation({
    mutationFn: updateTableState,
    onSuccess: () => queryClient.invalidateQueries(["tableManagers"]),
  });

  const discountMutation = useMutation({
    mutationFn: updateDiscount,
    onSuccess: () => queryClient.invalidateQueries(["tableManagers"]),
  });

  const assignPlayerMutation = useMutation({
    mutationFn: async (playerId: string) => {
      await axios.put(`http://localhost:5267/api/tablemanager/${selectedTableId}/session/player/${playerId}/update`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["tableManagers"]);
      setShowDiscountModal(false);
      setSelectedPlayerId(null);
      setPlayerFilters({ name: "", surname: "", email: "" });
    },
  });

  const applyPendingDiscountIfAny = async (tableId: number) => {
    const discount = pendingDiscounts[tableId];
    if (discount) {
      await discountMutation.mutateAsync({ tableId, discount });
      setPendingDiscounts((prev) => {
        const copy = { ...prev };
        delete copy[tableId];
        return copy;
      });
    }
  };

  const handleStateChange = async (tableId: number, newState: string) => {
    if (newState === "play") {
      const timedSeconds = parseInt(timedSessions[tableId] || "0") * 60;
      await stateMutation.mutateAsync({ tableId, newState, timedSeconds });
      await applyPendingDiscountIfAny(tableId);
      setTimedSessions((prev) => ({ ...prev, [tableId]: "" }));
    } else {
      stateMutation.mutate({ tableId, newState });
    }
  };

  const openDiscountModal = (tableId: number) => {
    setSelectedTableId(tableId);
    setShowDiscountModal(true);
    setDiscountModalTab("discounts");
  };

  const applyDiscount = (discount: Discount) => {
    if (selectedTableId !== null) {
      setPendingDiscounts((prev) => ({ ...prev, [selectedTableId]: discount }));
      setShowDiscountModal(false);
      const table = data.find((t: Table) => t.tableId === selectedTableId);
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
  if (isLoading || !data)
    return <p className="text-center text-gray-500 dark:text-gray-400">Loading tables...</p>;
  if (data.length === 0)
    return <p className="text-center text-gray-500 dark:text-gray-400">No tables found.</p>;

  const renderTableCard = (table: Table) => {
    let boxColor = "";
    switch (table.tableStatus.toLowerCase()) {
      case "play": boxColor = "bg-red-800"; break;
      case "standby": boxColor = "bg-blue-800"; break;
      case "off": boxColor = "bg-green-800"; break;
      default: boxColor = "bg-gray-500";
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
        <p className="text-2xl text-white mt-2"><span className="font-medium">Status:</span> {table.tableStatus}</p>
        <p className="text-2xl text-white"><span className="font-medium">Play Time:</span> {table.playTime}</p>
        <p className="text-2xl text-white"><span className="font-medium">Remaining:</span> {table.remainingTime}</p>

        {discount && (
          <p className="text-white text-lg mt-2">
            <span className="font-medium">Discount:</span> {discount.name} ({discount.rate}%)
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
            onChange={(e) => setTimedSessions((prev) => ({ ...prev, [table.tableId]: e.target.value }))}
            placeholder="Minutes (optional)"
            className="w-full p-2 mt-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-black dark:text-white"
          />
        )}

        <div className="mt-4 flex flex-col space-y-2">
          {table.tableStatus.toLowerCase() === "off" && (
            <ActionButton onClick={() => handleStateChange(table.tableId, "play")} colorClasses="dark:text-green-400 text-green-700">
              Play
            </ActionButton>
          )}
          {table.tableStatus.toLowerCase() === "play" && (
            <ActionButton onClick={() => handleStateChange(table.tableId, "standby")} colorClasses="dark:text-blue-400 text-blue-700">
              Standby
            </ActionButton>
          )}
          {table.tableStatus.toLowerCase() === "standby" && (
            <>
              <ActionButton onClick={() => handleStateChange(table.tableId, "play")} colorClasses="dark:text-green-400 text-green-700">
                Play
              </ActionButton>
              <ActionButton onClick={() => handleStateChange(table.tableId, "off")} colorClasses="dark:text-red-400 text-red-700">
                Off
              </ActionButton>
            </>
          )}
        </div>
      </div>
    );
  };

  const firstRow = data.slice(0, 5);
  const remainingTables = data.slice(5);

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

      {showDiscountModal && (
        <div className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50" role="dialog" aria-modal="true">
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg w-[600px] shadow-lg space-y-4">
            <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">Session Options</h2>

            <div className="flex space-x-2">
              <button
                onClick={() => setDiscountModalTab("discounts")}
                className={`px-4 py-2 rounded-t ${discountModalTab === "discounts" ? "bg-blue-600 text-white" : "bg-gray-300 dark:bg-gray-700 text-black dark:text-white"}`}
              >
                Select Discount
              </button>
              <button
                onClick={() => setDiscountModalTab("players")}
                className={`px-4 py-2 rounded-t ${discountModalTab === "players" ? "bg-blue-600 text-white" : "bg-gray-300 dark:bg-gray-700 text-black dark:text-white"}`}
              >
                Assign Player
              </button>
            </div>

            <div className="border border-t-0 rounded-b p-4 dark:border-gray-600 bg-gray-100 dark:bg-gray-900 max-h-[400px] overflow-y-auto">
              {discountModalTab === "discounts" && (
                <>
                  {isLoadingDiscounts ? (
                    <p className="text-gray-600 dark:text-gray-300">Loading discounts...</p>
                  ) : (
                    discounts?.map((discount: Discount) => (
                      <button
                        key={discount.id}
                        onClick={() => applyDiscount(discount)}
                        className="w-full text-left px-4 py-2 rounded hover:bg-gray-200 dark:hover:bg-gray-700 text-gray-900 dark:text-white border border-gray-300 dark:border-gray-600 mb-2"
                      >
                        {discount.name} ({discount.rate}%)
                      </button>
                    ))
                  )}
                </>
              )}

              {discountModalTab === "players" && (
                <div className="space-y-4">
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-2">
                    <input
                      placeholder="Name"
                      value={playerFilters.name}
                      onChange={(e) => setPlayerFilters({ ...playerFilters, name: e.target.value })}
                      className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
                    />
                    <input
                      placeholder="Surname"
                      value={playerFilters.surname}
                      onChange={(e) => setPlayerFilters({ ...playerFilters, surname: e.target.value })}
                      className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
                    />
                    <input
                      placeholder="Email"
                      value={playerFilters.email}
                      onChange={(e) => setPlayerFilters({ ...playerFilters, email: e.target.value })}
                      className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
                    />
                  </div>

                  {isSearchingPlayers ? (
                    <p className="text-gray-500">Searching players...</p>
                  ) : players?.length > 0 ? (
                    <ul className="space-y-2 max-h-48 overflow-auto">
                      {players.map((player: any) => (
                        <li
                          key={player.id}
                          onClick={() => setSelectedPlayerId(player.id)}
                          className={`p-2 border rounded cursor-pointer hover:bg-blue-100 dark:hover:bg-gray-700 ${
                            selectedPlayerId === player.id ? "bg-green-100 dark:bg-green-800" : ""
                          }`}
                        >
                          <div className="font-medium">{player.name} {player.surname}</div>
                          <div className="text-sm text-gray-500">{player.email}</div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p>No players found.</p>
                  )}

                  <button
                    disabled={!selectedPlayerId || assignPlayerMutation.isPending}
                    onClick={() => selectedPlayerId && assignPlayerMutation.mutate(selectedPlayerId)}
                    className="w-full bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 disabled:opacity-50"
                  >
                    {assignPlayerMutation.isPending ? "Assigning..." : "Assign Selected Player"}
                  </button>
                </div>
              )}
            </div>

            <button
              onClick={() => setShowDiscountModal(false)}
              className="mt-4 w-full bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

