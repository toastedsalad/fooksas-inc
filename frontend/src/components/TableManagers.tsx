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

type Player = {
  id: string;
  name: string;
  surname: string;
  email: string;
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

const updatePlayer = async ({
  tableId,
  playerId,
}: {
  tableId: number;
  playerId: string;
}) => {
  await axios.put(`http://localhost:5267/api/tablemanager/${tableId}/session/player/${playerId}/update`);
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
  const [pendingPlayers, setPendingPlayers] = useState<{ [key: number]: Player | null }>({});
  const [selectedTableId, setSelectedTableId] = useState<number | null>(null);
  const [showDiscountModal, setShowDiscountModal] = useState(false);
  const [discountModalTab, setDiscountModalTab] = useState<"discounts" | "players">("discounts");
  const [playerFilters, setPlayerFilters] = useState({ name: "", surname: "", email: "" });

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

  const playerMutation = useMutation({
    mutationFn: updatePlayer,
    onSuccess: () => {
      queryClient.invalidateQueries(["tableManagers"]);
      setShowDiscountModal(false);
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

  const applyPendingPlayerIfAny = async (tableId: number) => {
    const player = pendingPlayers[tableId];
    if (player) {
      await playerMutation.mutateAsync({ tableId, playerId: player.id });
      setPendingPlayers((prev) => {
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
      await applyPendingPlayerIfAny(tableId);
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

  const applyPlayer = (player: Player) => {
    if (selectedTableId !== null) {
      const table = data.find((t: Table) => t.tableId === selectedTableId);
      if (!table) return;

      if (table.tableStatus.toLowerCase() === "off") {
        // Save pending player and close modal, assign when session starts
        setPendingPlayers((prev) => ({ ...prev, [selectedTableId]: player }));
        setShowDiscountModal(false);
      } else {
        // Assign immediately and close modal
        playerMutation.mutate({ tableId: selectedTableId, playerId: player.id });
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
    }

    const discount = pendingDiscounts[table.tableId];
    const pendingPlayer = pendingPlayers[table.tableId];

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
            <span className="font-medium">Discount:</span> {discount.name} ({discount.rate}%)
          </p>
        )}

        {pendingPlayer && (
          <p className="text-white text-lg mt-1">
            <span className="font-medium">Player:</span> {pendingPlayer.name} {pendingPlayer.surname}
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
              setTimedSessions((prev) => ({ ...prev, [table.tableId]: e.target.value }))
            }
            placeholder="Minutes (optional)"
            className="w-full p-2 mt-2 rounded-lg border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-black dark:text-white"
          />
        )}

        <div className="mt-4 flex flex-col space-y-2">
          {table.tableStatus.toLowerCase() === "off" && (
            <ActionButton
              onClick={() => handleStateChange(table.tableId, "play")}
              colorClasses="dark:text-green-400 text-green-700"
            >
              Play
            </ActionButton>
          )}
          {table.tableStatus.toLowerCase() === "play" && (
            <ActionButton
              onClick={() => handleStateChange(table.tableId, "standby")}
              colorClasses="dark:text-blue-400 text-blue-700"
            >
              Standby
            </ActionButton>
          )}
          {table.tableStatus.toLowerCase() === "standby" && (
            <>
              <ActionButton
                onClick={() => handleStateChange(table.tableId, "play")}
                colorClasses="dark:text-green-400 text-green-700"
              >
                Play
              </ActionButton>
              <ActionButton
                onClick={() => handleStateChange(table.tableId, "off")}
                colorClasses="dark:text-red-400 text-red-700"
              >
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
        <div
          className="fixed inset-0 bg-black bg-opacity-40 flex items-center justify-center z-50"
          role="dialog"
          aria-modal="true"
        >
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg w-[600px] shadow-lg space-y-4">
            <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">Session Options</h2>

            <div className="flex space-x-2">
              <button
                onClick={() => setDiscountModalTab("discounts")}
                className={`px-4 py-2 rounded-t ${
                  discountModalTab === "discounts"
                    ? "bg-blue-600 text-white"
                    : "bg-gray-300 dark:bg-gray-700 text-black dark:text-white"
                }`}
              >
                Select Discount
              </button>
              <button
                onClick={() => setDiscountModalTab("players")}
                className={`px-4 py-2 rounded-t ${
                  discountModalTab === "players"
                    ? "bg-blue-600 text-white"
                    : "bg-gray-300 dark:bg-gray-700 text-black dark:text-white"
                }`}
              >
                Assign Player
              </button>
            </div>

            <div className="border border-t-0 rounded-b p-4 dark:border-gray-600 bg-gray-100 dark:bg-gray-900 max-h-[400px] overflow-y-auto">
              {discountModalTab === "discounts" && (
                <>
                  {isLoadingDiscounts && <p>Loading discounts...</p>}
                  {!isLoadingDiscounts && discounts?.length === 0 && <p>No discounts available.</p>}
                  {!isLoadingDiscounts &&
                    discounts?.map((discount) => (
                      <div
                        key={discount.id}
                        className="cursor-pointer p-2 rounded hover:bg-blue-200 dark:hover:bg-blue-700"
                        onClick={() => applyDiscount(discount)}
                      >
                        {discount.name} ({discount.rate}%)
                      </div>
                    ))}
                </>
              )}

              {discountModalTab === "players" && (
                <>
                  <div className="mb-4 space-y-2">
                    <input
                      type="text"
                      placeholder="Name"
                      value={playerFilters.name}
                      onChange={(e) =>
                        setPlayerFilters((prev) => ({ ...prev, name: e.target.value }))
                      }
                      className="w-full p-2 rounded border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700"
                    />
                    <input
                      type="text"
                      placeholder="Surname"
                      value={playerFilters.surname}
                      onChange={(e) =>
                        setPlayerFilters((prev) => ({ ...prev, surname: e.target.value }))
                      }
                      className="w-full p-2 rounded border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700"
                    />
                    <input
                      type="email"
                      placeholder="Email"
                      value={playerFilters.email}
                      onChange={(e) =>
                        setPlayerFilters((prev) => ({ ...prev, email: e.target.value }))
                      }
                      className="w-full p-2 rounded border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700"
                    />
                  </div>
                  {isSearchingPlayers && <p>Searching players...</p>}
                  {!isSearchingPlayers && players?.length === 0 && <p>No players found.</p>}
                  {!isSearchingPlayers &&
                    players?.map((player) => (
                      <div
                        key={player.id}
                        className="cursor-pointer p-2 rounded hover:bg-blue-200 dark:hover:bg-blue-700"
                        onClick={() => applyPlayer(player)}
                      >
                        {player.name} {player.surname} - {player.email}
                      </div>
                    ))}
                </>
              )}
            </div>

            <button
              onClick={() => setShowDiscountModal(false)}
              className="mt-4 w-full bg-red-600 hover:bg-red-700 text-white py-2 rounded"
            >
              Close
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

