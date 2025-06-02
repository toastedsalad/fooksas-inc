import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useEffect, useState } from "react";

const apiBase = "http://localhost:5267/api/player";

type Player = {
  id: string;
  createdAt: string;
  name: string;
  surname: string;
  email: string;
  discountRate: number;
};

function useDarkMode() {
  const [isDark, setIsDark] = useState(false);
  useEffect(() => {
    const root = document.documentElement;
    const observer = new MutationObserver(() => {
      setIsDark(root.classList.contains("dark"));
    });
    observer.observe(root, { attributes: true, attributeFilter: ["class"] });
    setIsDark(root.classList.contains("dark"));
    return () => observer.disconnect();
  }, []);
  return isDark;
}

export default function PlayersPage() {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState({ name: "", surname: "", email: "" });
  const [newPlayer, setNewPlayer] = useState({ name: "", surname: "", email: "", discountRate: 0 });
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<Partial<Player>>({});
  const isDark = useDarkMode();

  const { data: players, isLoading } = useQuery({
    queryKey: ["players", filters],
    queryFn: async () => {
      const hasFilters = filters.name || filters.surname || filters.email;
  
      if (hasFilters) {
        const params = new URLSearchParams();
        if (filters.name) params.append("name", filters.name);
        if (filters.surname) params.append("surname", filters.surname);
        if (filters.email) params.append("email", filters.email);
        const { data } = await axios.get<Player[]>(`${apiBase}/search?${params}`);
        return data;
      } else {
        // No filters — get 10 recent players
        const { data } = await axios.get<Player[]>(`${apiBase}/all`);
        return data;
      }
    },
    keepPreviousData: true,
  });

  const addMutation = useMutation({
    mutationFn: async () => {
      const { data } = await axios.post(apiBase, newPlayer);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["players"]);
      setNewPlayer({ name: "", surname: "", email: "", discountRate: 0 });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await axios.delete(`${apiBase}/${id}`);
    },
    onSuccess: () => queryClient.invalidateQueries(["players"]),
  });

  const updateMutation = useMutation({
    mutationFn: async (player: Player) => {
      await axios.put(`${apiBase}/${player.id}`, player);
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["players"]);
      setEditingId(null);
    },
  });

  const handleInputChange = (key: keyof typeof filters, value: string) => {
    setFilters({ ...filters, [key]: value });
  };

  const startEdit = (player: Player) => {
    setEditingId(player.id);
    setEditForm({ ...player });
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditForm({});
  };

  return (
    <div className="p-6 max-w-5xl mx-auto text-gray-900 dark:text-gray-100 space-y-6">
      <h1 className="text-3xl font-bold">Manage Players</h1>

      {/* Add New Player */}
      <div className="border p-4 rounded bg-gray-50 dark:bg-gray-800 border-gray-300 dark:border-gray-700 space-y-4">
        <h2 className="text-xl font-semibold">Add New Player</h2>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <input
            placeholder="Name"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newPlayer.name}
            onChange={(e) => setNewPlayer({ ...newPlayer, name: e.target.value })}
          />
          <input
            placeholder="Surname"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newPlayer.surname}
            onChange={(e) => setNewPlayer({ ...newPlayer, surname: e.target.value })}
          />
          <input
            placeholder="Email"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newPlayer.email}
            onChange={(e) => setNewPlayer({ ...newPlayer, email: e.target.value })}
          />
          <input
            type="number"
            placeholder="Discount %"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newPlayer.discountRate}
            onChange={(e) =>
              setNewPlayer({ ...newPlayer, discountRate: parseFloat(e.target.value) || 0 })
            }
          />
        </div>
        <button
          onClick={() => addMutation.mutate()}
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded"
        >
          Add Player
        </button>
      </div>

      {/* Filters */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {["name", "surname", "email"].map((key) => (
          <input
            key={key}
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            placeholder={`Search by ${key}`}
            value={filters[key as keyof typeof filters]}
            onChange={(e) => handleInputChange(key as keyof typeof filters, e.target.value)}
          />
        ))}
      </div>

      {/* Player List */}
      {isLoading ? (
        <p>Loading players...</p>
      ) : (
        <div className="space-y-2">
          {players?.length === 0 ? (
            <p>No players found.</p>
          ) : (
            players.map((player) =>
              editingId === player.id ? (
                <div
                  key={player.id}
                  className="grid md:grid-cols-6 gap-2 items-center border p-2 rounded bg-gray-50 dark:bg-gray-800"
                >
                  <input
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.name}
                    onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
                  />
                  <input
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.surname}
                    onChange={(e) => setEditForm({ ...editForm, surname: e.target.value })}
                  />
                  <input
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.email}
                    onChange={(e) => setEditForm({ ...editForm, email: e.target.value })}
                  />
                  <input
                    type="number"
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.discountRate}
                    onChange={(e) =>
                      setEditForm({ ...editForm, discountRate: parseFloat(e.target.value) || 0 })
                    }
                  />
                  <button
                    onClick={() => updateMutation.mutate(editForm as Player)}
                    className="bg-blue-600 text-white px-2 py-1 rounded"
                  >
                    Save
                  </button>
                  <button
                    onClick={cancelEdit}
                    className="text-gray-600 dark:text-gray-300 hover:text-red-600"
                  >
                    Cancel
                  </button>
                </div>
              ) : (
                <div
                  key={player.id}
                  className="flex justify-between items-center border p-2 rounded bg-gray-50 dark:bg-gray-800"
                >
                  <div>
                    <p className="font-medium">
                      {player.name} {player.surname}
                    </p>
                    <p className="text-sm text-gray-600 dark:text-gray-400">{player.email}</p>
                    <p className="text-sm">Discount: {player.discountRate}%</p>
                    <p className="text-sm text-gray-500 dark:text-gray-400"> Added: {new Date(player.createdAt).toLocaleString("lt-LT", {
                      year: "numeric",
                      month: "2-digit",
                      day: "2-digit",
                      hour: "2-digit",
                      minute: "2-digit",
                      hour12: false,
                    })
                    .replace(",", "")} </p>
                  </div>
                  <div className="space-x-2">
                    <button
                      onClick={() => startEdit(player)}
                      className="text-blue-600 hover:text-blue-700 dark:text-blue-400"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => deleteMutation.mutate(player.id)}
                      className="text-red-600 hover:text-red-700 dark:text-red-400"
                    >
                      Delete
                    </button>
                  </div>
                </div>
              )
            )
          )}
        </div>
      )}
    </div>
  );
}

