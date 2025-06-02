import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useEffect, useState } from "react";

const apiBase = "http://localhost:5267/api/discount";

type Discount = {
  id: string;
  type: string;
  name: string;
  rate: number;
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

export default function DiscountsPage() {
  const queryClient = useQueryClient();
  const [filters, setFilters] = useState({ type: "", name: "" });
  const [newDiscount, setNewDiscount] = useState<Partial<Discount>>({ type: "", name: "", rate: 0 });
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<Partial<Discount>>({});
  const isDark = useDarkMode();

  const { data: discounts, isLoading } = useQuery({
    queryKey: ["discounts", filters],
    queryFn: async () => {
      const hasFilters = filters.type || filters.name;

      if (hasFilters) {
        const params = new URLSearchParams();
        if (filters.type) params.append("type", filters.type);
        if (filters.name) params.append("name", filters.name);
        const { data } = await axios.get<Discount[]>(`${apiBase}/search?${params}`);
        return data;
      } else {
        const { data } = await axios.get<Discount[]>(`${apiBase}/all`);
        return data;
      }
    },
    keepPreviousData: true,
  });

  const addMutation = useMutation({
    mutationFn: async () => {
      const { data } = await axios.post(apiBase, newDiscount);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["discounts"]);
      setNewDiscount({ type: "", name: "", rate: 0 });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await axios.delete(`${apiBase}/${id}`);
    },
    onSuccess: () => queryClient.invalidateQueries(["discounts"]),
  });

  const updateMutation = useMutation({
    mutationFn: async (discount: Discount) => {
      await axios.put(`${apiBase}/${discount.id}`, discount);
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["discounts"]);
      setEditingId(null);
    },
  });

  const handleInputChange = (key: keyof typeof filters, value: string) => {
    setFilters({ ...filters, [key]: value });
  };

  const startEdit = (discount: Discount) => {
    setEditingId(discount.id);
    setEditForm({ ...discount });
  };

  const cancelEdit = () => {
    setEditingId(null);
    setEditForm({});
  };

  return (
    <div className="p-6 max-w-5xl mx-auto text-gray-900 dark:text-gray-100 space-y-6">
      <h1 className="text-3xl font-bold">Manage Discounts</h1>

      {/* Add New Discount */}
      <div className="border p-4 rounded bg-gray-50 dark:bg-gray-800 border-gray-300 dark:border-gray-700 space-y-4">
        <h2 className="text-xl font-semibold">Add New Discount</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <input
            placeholder="Type"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newDiscount.type}
            onChange={(e) => setNewDiscount({ ...newDiscount, type: e.target.value })}
          />
          <input
            placeholder="Name"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newDiscount.name}
            onChange={(e) => setNewDiscount({ ...newDiscount, name: e.target.value })}
          />
          <input
            type="number"
            placeholder="Rate (%)"
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            value={newDiscount.rate ?? ""}
            onChange={(e) =>
              setNewDiscount({ ...newDiscount, rate: parseFloat(e.target.value) || 0 })
            }
          />
        </div>
        <button
          onClick={() => addMutation.mutate()}
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded"
        >
          Add Discount
        </button>
      </div>

      {/* Filters */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {["type", "name"].map((key) => (
          <input
            key={key}
            className="p-2 border rounded bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600"
            placeholder={`Search by ${key}`}
            value={filters[key as keyof typeof filters]}
            onChange={(e) => handleInputChange(key as keyof typeof filters, e.target.value)}
          />
        ))}
      </div>

      {/* Discount List */}
      {isLoading ? (
        <p>Loading discounts...</p>
      ) : (
        <div className="space-y-2">
          {discounts?.length === 0 ? (
            <p>No discounts found.</p>
          ) : (
            discounts.map((discount) =>
              editingId === discount.id ? (
                <div
                  key={discount.id}
                  className="grid md:grid-cols-5 gap-2 items-center border p-2 rounded bg-gray-50 dark:bg-gray-800"
                >
                  <input
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.type}
                    onChange={(e) => setEditForm({ ...editForm, type: e.target.value })}
                  />
                  <input
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.name}
                    onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
                  />
                  <input
                    type="number"
                    className="p-1 rounded bg-white dark:bg-gray-700 border border-gray-300 dark:border-gray-600"
                    value={editForm.rate}
                    onChange={(e) =>
                      setEditForm({ ...editForm, rate: parseFloat(e.target.value) || 0 })
                    }
                  />
                  <button
                    onClick={() => updateMutation.mutate(editForm as Discount)}
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
                  key={discount.id}
                  className="flex justify-between items-center border p-2 rounded bg-gray-50 dark:bg-gray-800"
                >
                  <div>
                    <p className="font-medium">
                      {discount.name} ({discount.type})
                    </p>
                    <p className="text-sm text-gray-600 dark:text-gray-400">Rate: {discount.rate}%</p>
                  </div>
                  <div className="space-x-2">
                    <button
                      onClick={() => startEdit(discount)}
                      className="text-blue-600 hover:text-blue-700 dark:text-blue-400"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => deleteMutation.mutate(discount.id)}
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

